// handles database operations for service requests, uses observer to validate contract status

using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Models;
using ST10438307_GLMS.Observers;

namespace ST10438307_GLMS.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ServiceRequestValidator _validator;
    private readonly AuditLogger _auditLogger;

    public ServiceRequestService(
        IDbContextFactory<AppDbContext> contextFactory,
        ServiceRequestValidator validator,
        AuditLogger auditLogger)
    {
        _contextFactory = contextFactory;
        _validator = validator;
        _auditLogger = auditLogger;
    }

    //Read Operations
    //-----------------------------------------------------------------------------------------------

    public async Task<List<ServiceRequest>> GetAllServiceRequestsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ServiceRequests
            .Include(sr => sr.Contract)
            .ThenInclude(c => c.Client) // need client name on the list view
            .ToListAsync();
    }

    public async Task<List<ServiceRequest>> GetServiceRequestsByContractIdAsync(int contractId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ServiceRequests
            .Include(sr => sr.Contract)
            .ThenInclude(c => c.Client)
            .Where(sr => sr.ContractId == contractId)
            .ToListAsync();
    }

    public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ServiceRequests
            .Include(sr => sr.Contract)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    //-----------------------------------------------------------------------------------------------

    //Write Operations
    //-----------------------------------------------------------------------------------------------

    public async Task AddServiceRequestAsync(ServiceRequest serviceRequest)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        //Observer Check - validate parent contract
        //-------------------------------------------------------
        var contract = await context.Contracts.FindAsync(serviceRequest.ContractId);
        if (contract == null)
            throw new InvalidOperationException("contract not found.");

        //Input Validation
        //-------------------------------------------------------
        if (string.IsNullOrWhiteSpace(serviceRequest.Description))
            throw new InvalidOperationException("description cannot be empty.");

        if (serviceRequest.CostZAR < 0)
            throw new InvalidOperationException("cost cannot be negative.");
        //-------------------------------------------------------

        contract.Attach(_validator);
        contract.Attach(_auditLogger);
        contract.Notify();

        if (_validator.IsBlocked) // flag set by observer based on contract status
            throw new InvalidOperationException(
                $"service requests cannot be raised against a contract with status '{contract.Status}'.");
        //-------------------------------------------------------

        context.ServiceRequests.Add(serviceRequest);
        await context.SaveChangesAsync();
    }

    public async Task UpdateServiceRequestAsync(ServiceRequest serviceRequest)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.ServiceRequests.Update(serviceRequest);
        await context.SaveChangesAsync();
    }

    public async Task UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus newStatus)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var sr = await context.ServiceRequests.FindAsync(id);
        if (sr == null) return;

        sr.Status = newStatus;
        await context.SaveChangesAsync();
    }

    public async Task DeleteServiceRequestAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var sr = await context.ServiceRequests.FindAsync(id);
        if (sr != null)
        {
            context.ServiceRequests.Remove(sr);
            await context.SaveChangesAsync();
        }
    }

    //-----------------------------------------------------------------------------------------------
}