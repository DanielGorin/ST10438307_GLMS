using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Observers;

// A paper trail for compliance
public class AuditLogger : IContractObserver
{
    public List<string> Log { get; private set; } = new();

    public void OnStatusChanged(Contract contract)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Contract {contract.Id} status changed to {contract.Status}";
        Log.Add(entry);
        Console.WriteLine(entry);
    }
}