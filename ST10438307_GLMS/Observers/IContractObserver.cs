using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Observers;

public interface IContractObserver
{
    void OnStatusChanged(Contract contract);
}