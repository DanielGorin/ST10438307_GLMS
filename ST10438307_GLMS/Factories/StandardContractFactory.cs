// creates a standard contract

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;

public class StandardContractFactory : IContractFactory
{
    public Contract CreateContract()
    {
        return new Contract
        {
            ServiceLevel = "Standard",
            Status = ContractStatus.Draft,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)// 1 years = standard contract default
        };
    }
}