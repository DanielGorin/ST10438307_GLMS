// currency service interface - defines rate fetch and conversion operations

namespace ST10438307_GLMS.Services;


public interface ICurrencyService
{
    Task<decimal> GetUsdToZarRateAsync();
    decimal ConvertUsdToZar(decimal usdAmount, decimal rate);
    decimal ConvertZarToUsd(decimal zarAmount, decimal rate);
}