using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Models;
using ST10438307_GLMS.Observers;

namespace ST10438307_GLMS.Services;

// directly accesse SQLite via DbContextFactory

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

    public async Task<List<ServiceRequest>> GetAllServiceRequestsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ServiceRequests
            .Include(sr => sr.Contract)
            .ThenInclude(c => c.Client)
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

    public async Task AddServiceRequestAsync(ServiceRequest serviceRequest)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // OBSERVER PATTERN: loads parrent contract and attaches observers
        //The ServiceRequestValidator checks whether the contract is expired or on hold
        var contract = await context.Contracts.FindAsync(serviceRequest.ContractId);
        if (contract == null)
            throw new InvalidOperationException("Contract not found.");

        contract.Attach(_validator);
        contract.Attach(_auditLogger);
        contract.Notify();

        if (_validator.IsBlocked)
            throw new InvalidOperationException(
                $"Cannot create a Service Request on a contract with status '{contract.Status}'. " +
                "Only Active or Draft contracts accept new Service Requests.");

        context.ServiceRequests.Add(serviceRequest);
        await context.SaveChangesAsync();
    }

    public async Task UpdateServiceRequestAsync(ServiceRequest serviceRequest)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.ServiceRequests.Update(serviceRequest);
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
}