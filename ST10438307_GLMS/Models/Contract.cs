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

    //PDF upload path
    public string? SignedAgreementPath { get; set; }

    public List<ServiceRequest> ServiceRequests { get; set; } = new();

    // Observer
    private List<IContractObserver> _observers = new();

    public void Attach(IContractObserver observer)
    {
        _observers.Add(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.OnStatusChanged(this);
        }
    }
}