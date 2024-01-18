using CurrencyConverter.Enums;
using Newtonsoft.Json;

namespace CurrencyConverter;

public class ExchangeRateAPIConverterModel
{
    [JsonProperty("time_last_update_utc")]
    public DateTime LastUpdate { get; init; }

    [JsonProperty("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; init; }
}