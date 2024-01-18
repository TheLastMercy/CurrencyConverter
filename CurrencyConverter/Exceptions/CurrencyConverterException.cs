namespace CurrencyConverter.Exceptions;

public class CurrencyConverterException : Exception
{
    public CurrencyConverterException()
    {
        
    }

    public CurrencyConverterException(string message, Exception? innerException = null) : base(message, innerException)
    {
        
    }
}