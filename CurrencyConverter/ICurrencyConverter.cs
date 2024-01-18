using CurrencyConverter.Enums;

namespace CurrencyConverter;

public interface ICurrencyConverter
{
    DateTime ValidityDate { get; } 
    IEnumerable<CurrencyCode> SupportedCurrencies { get; }
    
    decimal ConversionRate(CurrencyCode source, CurrencyCode destination);
    
    decimal Convert(CurrencyCode source, CurrencyCode destination, decimal amount);
}
