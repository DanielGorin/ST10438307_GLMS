// Service request entity - belongs to a contract stores cost in USD and Zar

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

    [Required]
    public decimal CostUSD { get; set; } // user entered

    public decimal CostZAR { get; set; } // calculated by the currency service

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}