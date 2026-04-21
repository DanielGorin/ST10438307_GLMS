// observer - Bocks new service requests if the contract is expired or on hold

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Observers;

public class ServiceRequestValidator : IContractObserver
{
    public bool IsBlocked { get; private set; } = false; // checked by the service before saving

    public void OnStatusChanged(Contract contract)
    {
        //Status Check - set blocked based on contract status
        //-------------------------------------------------------
        IsBlocked = contract.Status == ContractStatus.Expired ||
                    contract.Status == ContractStatus.OnHold;
        //-------------------------------------------------------
    }
}