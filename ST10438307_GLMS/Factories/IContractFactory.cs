// Abstract Factory Interface - all contract factores must implement this

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Factories;

public interface IContractFactory
{
    Contract CreateContract();
}