// contract entity - belongs to a client tracks status and service level

using System.ComponentModel.DataAnnotations;
using ST10438307_GLMS.Observers;

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

    public string? SignedAgreementPath { get; set; }

    // EF uses this to join service requests back to their contract
    public List<ServiceRequest> ServiceRequests { get; set; } = new();

    //Observer Support - private EF doesnt map it to a column
    //-------------------------------------------------------------------------------------------------
    private List<IContractObserver> _observers = new();

    public void Attach(IContractObserver observer)
    {
        _observers.Add(observer);
    }

    public void Notify() // fires on status changes calling attached observers
    {
        foreach (var observer in _observers)
            observer.OnStatusChanged(this);
    }
    //-----------------------------------------------------------------------------------------------
}