// observer - records status changes

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Observers;

public class AuditLogger : IContractObserver
{
    public List<string> Log { get; private set; } = new();

    public void OnStatusChanged(Contract contract)
    {
        //Log Entry - stamp with time, contract id and new status
        //-------------------------------------------------------
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] contract {contract.Id} status is now {contract.Status}";
        Log.Add(entry);
        Console.WriteLine(entry);
        //-------------------------------------------------------
    }
}