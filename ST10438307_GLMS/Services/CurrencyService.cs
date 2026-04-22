// fetches live USD to ZAR rate and handles conversions

using System.Text.Json;

namespace ST10438307_GLMS.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //Rate Fetch - one call per page loads
    //-----------------------------------------------------------------------------------------------
    public async Task<decimal> GetUsdToZarRateAsync()
    {
        try
        {
            var url = "https://open.er-api.com/v6/latest/USD";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var rate = doc.RootElement
                .GetProperty("rates")
                .GetProperty("ZAR")
                .GetDecimal();

            return rate;
        }
        catch
        {
            return 18.50m; // fallback if the api is unreachable
        }
    }
    //-----------------------------------------------------------------------------------------------

    //Conversions - both directions using fetched rate
    //-----------------------------------------------------------------------------------------------
    public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
    {
        return Math.Round(usdAmount * rate, 2);
    }

    public decimal ConvertZarToUsd(decimal zarAmount, decimal rate)
    {
        if (rate == 0) return 0;
        return Math.Round(zarAmount / rate, 2);
    }
    //-----------------------------------------------------------------------------------------------
}