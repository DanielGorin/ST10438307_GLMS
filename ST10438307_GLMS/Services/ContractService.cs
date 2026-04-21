// Handles all database operations for contracts

using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Factories;
using ST10438307_GLMS.Models;
using ST10438307_GLMS.Observers;

namespace ST10438307_GLMS.Services;

public class ContractService : IContractService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly StandardContractFactory _standardFactory;
    private readonly SLAContractFactory _slaFactory;
    private readonly InternationalContractFactory _internationalFactory;
    private readonly AuditLogger _auditLogger;

    public ContractService(
        IDbContextFactory<AppDbContext> contextFactory,
        StandardContractFactory standardFactory,
        SLAContractFactory slaFactory,
        InternationalContractFactory internationalFactory,
        AuditLogger auditLogger)
    {
        _contextFactory = contextFactory;
        _standardFactory = standardFactory;
        _slaFactory = slaFactory;
        _internationalFactory = internationalFactory;
        _auditLogger = auditLogger;
    }

    //Read Operations - fetch contracts from the database
    //-----------------------------------------------------------------------------------------------

    public async Task<List<Contract>> GetAllContractsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts
            .Include(c => c.Client) // pulls client name in the same query
            .ToListAsync();
    }

    public async Task<List<Contract>> GetFilteredContractsAsync(
        DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Contracts.Include(c => c.Client).AsQueryable();

        //Filter application - only applies filters the user actually set
        //-------------------------------------------------------
        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);
        //-------------------------------------------------------

        return await query.ToListAsync();
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    //-----------------------------------------------------------------------------------------------

    //Write Operations - create, update and delete contracts
    //-----------------------------------------------------------------------------------------------

    public async Task AddContractAsync(Contract contract, string contractType)
    {
        //Abstract Factory - picks the right factory based on what type was selected
        //-------------------------------------------------------
        IContractFactory factory = contractType switch
        {
            "SLA" => _slaFactory,
            "International" => _internationalFactory,
            _ => _standardFactory // default to standard
        };
        //-------------------------------------------------------

        var newContract = factory.CreateContract(); // factory sets service level and default dates
        newContract.ClientId = contract.ClientId;
        newContract.StartDate = contract.StartDate;
        newContract.EndDate = contract.EndDate;

        //Observer - log the creation event
        //-------------------------------------------------------
        newContract.Attach(_auditLogger);
        newContract.Notify();
        //-------------------------------------------------------

        using var context = await _contextFactory.CreateDbContextAsync();
        context.Contracts.Add(newContract);
        await context.SaveChangesAsync();
    }

    public async Task UpdateContractAsync(Contract contract)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Contracts.Update(contract);
        await context.SaveChangesAsync();
    }

    public async Task UpdateContractStatusAsync(int id, ContractStatus newStatus)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var contract = await context.Contracts.FindAsync(id);
        if (contract == null) return;

        contract.Status = newStatus;

        //Observer - notify audit logger every time a status changes
        //-------------------------------------------------------
        contract.Attach(_auditLogger);
        contract.Notify();
        //-------------------------------------------------------

        await context.SaveChangesAsync();
    }

    public async Task UpdateSignedAgreementPathAsync(int id, string path)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var contract = await context.Contracts.FindAsync(id);
        if (contract == null) return;

        contract.SignedAgreementPath = path; // store relative path for URL
        await context.SaveChangesAsync();
    }


    public async Task DeleteContractAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var contract = await context.Contracts.FindAsync(id);
        if (contract != null)
        {
            context.Contracts.Remove(contract);
            await context.SaveChangesAsync();
        }
    }

    //-----------------------------------------------------------------------------------------------
}