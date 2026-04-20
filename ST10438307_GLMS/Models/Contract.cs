using System.ComponentModel.DataAnnotations;

namespace ST10438307_GLMS.Models;

public enum ContractStatus
{
    Draft,
    Active,
    Expired,
    OnHold
}

public class Contract
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

    [Required]
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    [Required]
    public string ServiceLevel { get; set; } = string.Empty;

    //For PDF Uplaods
    public string? SignedAgreementPath { get; set; } 

    // 1 Contract many Service Requests
    public List<ServiceRequest> ServiceRequests { get; set; } = new();
}