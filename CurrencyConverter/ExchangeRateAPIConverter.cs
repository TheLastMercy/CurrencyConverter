using CurrencyConverter.Enums;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using CurrencyConverter.Enums;
using CurrencyConverter.Exceptions;
using Newtonsoft.Json;

namespace CurrencyConverter;
// https://restsharp.dev
// https://www.exchangerate-api.com/docs/overview
// 58d47103f45b7227c3e74f80
public class ExchangeRateAPIConverter : ICurrencyConverter
{
    public DateTime ValidityDate { get; set; }

    public IEnumerable<CurrencyCode> SupportedCurrencies => _rates.Keys;
    
    private IDictionary<CurrencyCode, decimal> _rates = new Dictionary<CurrencyCode, decimal>();
    
    private RestClient _restClient;

    public ExchangeRateAPIConverter(string apiKey)
    {
        _restClient = new RestClient(
            $"https://v6.exchangerate-api.com/v6/{apiKey}/",
            configureSerialization: s => s.UseNewtonsoftJson()
            );
        
        var request = new RestRequest("latest/USD");
        var response = _restClient.GetAsync(request).Result;
        
        if (response.IsSuccessful)
        {
            var result = JsonConvert.DeserializeObject<ExchangeRateAPIConverterModel>(response.Content);
            ValidityDate = result.LastUpdate;
            _rates = result.ConversionRates.ToDictionary(
                keyValuePair =>
                {
                    if (Enum.TryParse(keyValuePair.Key, out CurrencyCode currencyCode))
                    {
                        return currencyCode;
                    }
                    else
                    {
                        
                        throw new CurrencyConverterException($"Failed to parse currency code: {keyValuePair.Key}");
                    }
                },
                keyValuePair => keyValuePair.Value);
        }
        else
        {
            throw new CurrencyConverterException("API request error");
        }
        
    }
    

    public decimal ConversionRate(CurrencyCode source, CurrencyCode destination)
    {
        if (!SupportedCurrencies.Contains(source) || !SupportedCurrencies.Contains(destination))
        {
            throw new CurrencyConverterException("Unsupported currency");
        }
        
        return 1 / _rates[source] * _rates[destination];
    }

    public decimal Convert(CurrencyCode source, CurrencyCode destination, decimal amount)
    {
        return ConversionRate(source, destination) * amount;
    }
}