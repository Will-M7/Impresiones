using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;

namespace Impresiones.Domain.ValueObjects;

public sealed record PrintSettings
{
    public const int MaximumCopies = 999;

    public PrintSettings(
        PaperSize paperSize,
        ColorMode colorMode,
        SidesMode sidesMode,
        PageOrientation orientation,
        int copies)
    {
        EnsureDefined(paperSize, nameof(paperSize));
        EnsureDefined(colorMode, nameof(colorMode));
        EnsureDefined(sidesMode, nameof(sidesMode));
        EnsureDefined(orientation, nameof(orientation));

        if (copies <= 0)
        {
            throw new DomainRuleException("Copies must be greater than zero.");
        }

        if (copies > MaximumCopies)
        {
            throw new DomainRuleException($"Copies must be less than or equal to {MaximumCopies}.");
        }

        PaperSize = paperSize;
        ColorMode = colorMode;
        SidesMode = sidesMode;
        Orientation = orientation;
        Copies = copies;
    }

    public PaperSize PaperSize { get; }

    public ColorMode ColorMode { get; }

    public SidesMode SidesMode { get; }

    public PageOrientation Orientation { get; }

    public int Copies { get; }

    private static void EnsureDefined<TEnum>(TEnum value, string optionName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new DomainRuleException($"{optionName} has an unsupported value.");
        }
    }
}
