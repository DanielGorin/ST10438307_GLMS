using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Factories;
using ST10438307_GLMS.Models;
using ST10438307_GLMS.Observers;

namespace ST10438307_GLMS.Services;

// Accesses SQLite via DbContextFactory
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

    public async Task<List<Contract>> GetAllContractsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts
            .Include(c => c.Client)
            .ToListAsync();
    }

    // filter by date,range and status
    public async Task<List<Contract>> GetFilteredContractsAsync(
        DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Contracts.Include(c => c.Client).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Abstract Factory Pattern: use factory based on contract type
    public async Task AddContractAsync(Contract contract, string contractType)
    {
        IContractFactory factory = contractType switch
        {
            "SLA" => _slaFactory,
            "International" => _internationalFactory,
            _ => _standardFactory
        };

        var newContract = factory.CreateContract();
        newContract.ClientId = contract.ClientId;
        newContract.StartDate = contract.StartDate;
        newContract.EndDate = contract.EndDate;
        newContract.ServiceLevel = newContract.ServiceLevel;

        // OBSERVER PATERN: attach the audit logger
        newContract.Attach(_auditLogger);
        newContract.Notify();

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

    // OBSERVER PATERN: notifies observers about status changes
    public async Task UpdateContractStatusAsync(int id, ContractStatus newStatus)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var contract = await context.Contracts.FindAsync(id);
        if (contract == null) return;

        contract.Status = newStatus;

        // atach and notify observers
        contract.Attach(_auditLogger);
        contract.Notify();

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
}