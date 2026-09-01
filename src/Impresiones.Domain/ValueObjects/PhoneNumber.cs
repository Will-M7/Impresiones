using Impresiones.Domain.Exceptions;

namespace Impresiones.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public const int RequiredLength = 9;

    public PhoneNumber(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
        {
            throw new DomainRuleException("Phone number is required.");
        }

        if (value.Length != RequiredLength)
        {
            throw new DomainRuleException("Phone number must contain exactly nine digits.");
        }

        if (!value.All(static character => character is >= '0' and <= '9'))
        {
            throw new DomainRuleException("Phone number must contain only ASCII digits.");
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}
