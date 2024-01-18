// See https://aka.ms/new-console-template for more information

using CurrencyConverter.Enums;
using CurrencyConverter.Exceptions;

namespace CurrencyConverter;

public class Program
{
    
    public static void Main(string[] args)
    {
        ICurrencyConverter converter;
        
        Console.WriteLine("Choose a converter:");
        Console.WriteLine("Input only the number of the converter.");
        Console.WriteLine("1 - CNB");
        Console.WriteLine("2 - ExchangeRateAPI");

        var choice = Convert.ToInt32(Console.ReadLine());
        
        switch (choice)
        {
            case 1:
                converter = new CNBCurrencyConverter();
                break;
            case 2:
                Console.WriteLine("Enter your ExchangeRateAPI API key:");
                string apiKey = Console.ReadLine();
                converter = new ExchangeRateAPIConverter(apiKey);
                break;
            default:
                Console.WriteLine("Invalid choice. Exiting...");
                return;
        }

        try
        {
            Console.WriteLine(converter.Convert(CurrencyCode.EUR, CurrencyCode.CZK, 1));
            Console.WriteLine(converter.Convert(CurrencyCode.EUR, CurrencyCode.CZK, 1));
            Console.WriteLine(converter.Convert(CurrencyCode.HUF, CurrencyCode.CZK, 100));
            Console.WriteLine(converter.Convert(CurrencyCode.USD, CurrencyCode.GBP, 1));
            Console.WriteLine(converter.Convert(CurrencyCode.EUR, CurrencyCode.USD, 1));
            Console.WriteLine(converter.Convert(CurrencyCode.RUB, CurrencyCode.CZK, 100));
        }
        catch (Exception e)
        {
            throw new CurrencyConverterException("Unable to refresh exchange rates", e);
        }
    }
}