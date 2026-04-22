// service request entity - belongs to a contract, stores cost in zar

using System.ComponentModel.DataAnnotations;

namespace ST10438307_GLMS.Models;

public enum ServiceRequestStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public class ServiceRequest
{
    public int Id { get; set; }

    [Required]
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    [Required]
    public string Description { get; set; } = string.Empty;

    public decimal CostZAR { get; set; }

    [Required]
    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}