namespace Impresiones.Infrastructure.Configuration;

public sealed class DataPathConfigurationException : InvalidOperationException
{
    public DataPathConfigurationException(string message)
        : base(message)
    {
    }

    public DataPathConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
