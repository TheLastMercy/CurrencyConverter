using CsvHelper.Configuration.Attributes;
using CurrencyConverter.Enums;

namespace CurrencyConverter;

public class CNBExchangeRateEntry
{
    [Index(2)]
    public decimal Amount { get; init; }
    
    [Index(3)]
    public CurrencyCode CurrencyCode { get; init; }
    
    [Index(4)]
    public decimal Rate { get; init; }
}