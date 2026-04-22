// ServicRequest interface defines crud operations for service requests

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;

public interface IServiceRequestService
{
    Task<List<ServiceRequest>> GetAllServiceRequestsAsync();
    Task<List<ServiceRequest>> GetServiceRequestsByContractIdAsync(int contractId);
    Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);
    Task AddServiceRequestAsync(ServiceRequest serviceRequest);
    Task UpdateServiceRequestAsync(ServiceRequest serviceRequest);
    Task UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus newStatus);
    Task DeleteServiceRequestAsync(int id);
}