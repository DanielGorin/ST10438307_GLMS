using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;

// This interface stays in place only the implementation changes

public interface IContractService
{
    Task<List<Contract>> GetAllContractsAsync();
    Task<List<Contract>> GetFilteredContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
    Task<Contract?> GetContractByIdAsync(int id);
    Task AddContractAsync(Contract contract, string contractType);
    Task UpdateContractAsync(Contract contract);
    Task UpdateContractStatusAsync(int id, ContractStatus newStatus);
    Task DeleteContractAsync(int id);
}