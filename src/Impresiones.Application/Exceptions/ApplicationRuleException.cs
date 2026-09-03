namespace Impresiones.Application.Exceptions;

public sealed class ApplicationRuleException : InvalidOperationException
{
    public ApplicationRuleException(string message)
        : base(message)
    {
    }
}
