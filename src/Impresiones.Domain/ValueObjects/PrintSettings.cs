using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;

namespace Impresiones.Domain.ValueObjects;

public sealed record PrintSettings
{
    public const int MaximumCopies = 999;

    private PaperSize _paperSize;
    private ColorMode _colorMode;
    private SidesMode _sidesMode;
    private PageOrientation _orientation;
    private int _copies;

    public PrintSettings(
        PaperSize paperSize,
        ColorMode colorMode,
        SidesMode sidesMode,
        PageOrientation orientation,
        int copies)
    {
        PaperSize = paperSize;
        ColorMode = colorMode;
        SidesMode = sidesMode;
        Orientation = orientation;
        Copies = copies;
    }

    public static PrintSettings Default => new(
        PaperSize.A4,
        ColorMode.BlackAndWhite,
        SidesMode.SingleSided,
        PageOrientation.Portrait,
        1);

    public PaperSize PaperSize
    {
        get => _paperSize;
        init
        {
            EnsureDefined(value, nameof(PaperSize));
            _paperSize = value;
        }
    }

    public ColorMode ColorMode
    {
        get => _colorMode;
        init
        {
            EnsureDefined(value, nameof(ColorMode));
            _colorMode = value;
        }
    }

    public SidesMode SidesMode
    {
        get => _sidesMode;
        init
        {
            EnsureDefined(value, nameof(SidesMode));
            _sidesMode = value;
        }
    }

    public PageOrientation Orientation
    {
        get => _orientation;
        init
        {
            EnsureDefined(value, nameof(Orientation));
            _orientation = value;
        }
    }

    public int Copies
    {
        get => _copies;
        init
        {
            if (value <= 0)
            {
                throw new DomainRuleException("Copies must be greater than zero.");
            }

            if (value > MaximumCopies)
            {
                throw new DomainRuleException($"Copies must be less than or equal to {MaximumCopies}.");
            }

            _copies = value;
        }
    }

    public PrintSettings Copy()
    {
        return this with { };
    }

    private static void EnsureDefined<TEnum>(TEnum value, string optionName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new DomainRuleException($"{optionName} has an unsupported value.");
        }
    }
}
