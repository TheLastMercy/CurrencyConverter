using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CurrencyConverter.Enums;
using CurrencyConverter.Exceptions;

namespace CurrencyConverter;

public class CNBCurrencyConverter : ICurrencyConverter
{
    public DateTime ValidityDate { get; private set; }
    public IEnumerable<CurrencyCode> SupportedCurrencies => _rates.Keys;

    private IDictionary<CurrencyCode, decimal> _rates = new Dictionary<CurrencyCode, decimal>();
    public CNBCurrencyConverter()
    {
        Refresh();
    }
    
    public void Refresh()
    {
        try
        {
            using (var client = new HttpClient())
            {
                using (var result = client.GetAsync("https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt").Result)
                {
                    if (result.IsSuccessStatusCode)
                    {
                        var response = result.Content.ReadAsStringAsync().Result;
                        ValidityDate = DateTime.ParseExact(response.Substring(0, 10), "dd.MM.yyyy", null).AddHours(14).AddMinutes(30);
                        // byte[] byteArray = Encoding.ASCII.GetBytes(response);
                        // MemoryStream stream = new MemoryStream(byteArray);
                        
                        var config = new CsvConfiguration(new CultureInfo("cs-CZ"))
                        {
                            Delimiter = "|",
                            Encoding = Encoding.UTF8,
                            // lambda expression
                            ShouldSkipRecord = ctx =>ctx.Row.Parser.Row == 1 || ctx.Row.Parser.Row == 2
                        };

                        // Clear/Empty dictionary
                        _rates = new Dictionary<CurrencyCode, decimal>();
                        
    
                        // using (var reader = new StreamReader(stream))
                        using (var csv = new CsvReader(new StringReader(response), config))
                        {
                            var records = csv.GetRecords<CNBExchangeRateEntry>();
                            
                            foreach (var record in records)
                            {
                                _rates.Add(record.CurrencyCode, record.Rate / record.Amount);
                            }
                            
                            // virtual exchange rate
                            _rates.Add(CurrencyCode.CZK, 1); // viz. metoda ConversionRate, zjednodušení
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new CurrencyConverterException("Unable to refresh exchange rates", e);
        }
    }
    
    public decimal ConversionRate(CurrencyCode source, CurrencyCode destination)
    {
        if (!SupportedCurrencies.Contains(source) || !SupportedCurrencies.Contains(destination))
        {
            throw new CurrencyConverterException("Unsupported currency");
        }
        
        return _rates[source] / _rates[destination];
    }
    
    public decimal Convert(CurrencyCode source, CurrencyCode destination, decimal amount)
    {
        return ConversionRate(source, destination) * amount;
    }

    
}