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

    // $ Cost
    [Required]
    public decimal CostUSD { get; set; }

    // R Cost
    public decimal CostZAR { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}