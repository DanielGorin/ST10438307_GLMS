// contract service interface - defines crud and filter operations for contracts

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;

public interface IContractService
{
    Task<List<Contract>> GetAllContractsAsync();
    Task<List<Contract>> GetFilteredContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
    Task<Contract?> GetContractByIdAsync(int id);
    Task AddContractAsync(Contract contract, string contractType);
    Task UpdateContractAsync(Contract contract);
    Task UpdateContractStatusAsync(int id, ContractStatus newStatus);
    Task UpdateSignedAgreementPathAsync(int id, string path); // saves the uploaded pdf path
    Task DeleteContractAsync(int id);
}