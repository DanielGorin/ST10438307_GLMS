// service request entity - belongs to a contract, stores cost in ZAR

using System.ComponentModel.DataAnnotations;

namespace ST10438307_GLMS.Models;

public class ServiceRequest
{
    public int Id { get; set; }

    [Required]
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    [Required]
    public string Description { get; set; } = string.Empty;

    public decimal CostZAR { get; set; } // Stored in ZAR

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}