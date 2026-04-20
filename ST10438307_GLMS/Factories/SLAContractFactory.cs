using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;

//Create SLA contracts with guaranteed service levels
public class SLAContractFactory : IContractFactory
{
    public Contract CreateContract()
    {
        return new Contract
        {
            ServiceLevel = "SLA",
            Status = ContractStatus.Draft,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(2)
        };
    }
}