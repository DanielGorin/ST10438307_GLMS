using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;


// Controllers and services depend on this, not concrete factories
public interface IContractFactory
{
    Contract CreateContract();
}