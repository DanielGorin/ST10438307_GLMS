using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Observers;

// block new ServiceRequests if Contract is Expired or OnHold
public class ServiceRequestValidator : IContractObserver
{
    public bool IsBlocked { get; private set; } = false;

    public void OnStatusChanged(Contract contract)
    {
        if (contract.Status == ContractStatus.Expired ||
            contract.Status == ContractStatus.OnHold)
        {
            IsBlocked = true;
        }
        else
        {
            IsBlocked = false;
        }
    }
}