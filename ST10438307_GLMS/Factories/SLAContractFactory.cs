// creates SLA contract

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;

public class SLAContractFactory : IContractFactory
{
    public Contract CreateContract()
    {
        return new Contract
        {
            ServiceLevel = "SLA",
            Status = ContractStatus.Draft,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
    }
}