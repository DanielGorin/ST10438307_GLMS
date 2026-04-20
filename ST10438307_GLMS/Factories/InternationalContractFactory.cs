using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;

//  creates international contracts requiring currency conversion
public class InternationalContractFactory : IContractFactory
{
    public Contract CreateContract()
    {
        return new Contract
        {
            ServiceLevel = "International",
            Status = ContractStatus.Draft,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(3)
        };
    }
}