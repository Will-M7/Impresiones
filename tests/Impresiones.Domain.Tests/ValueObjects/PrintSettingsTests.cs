using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Tests.ValueObjects;

public class PrintSettingsTests
{
    [Fact]
    public void Constructor_CreatesValidSettings()
    {
        var settings = CreateSettings();

        Assert.Equal(PaperSize.A4, settings.PaperSize);
        Assert.Equal(ColorMode.BlackAndWhite, settings.ColorMode);
        Assert.Equal(SidesMode.SingleSided, settings.SidesMode);
        Assert.Equal(PageOrientation.Portrait, settings.Orientation);
        Assert.Equal(1, settings.Copies);
    }

    [Fact]
    public void Equals_UsesValueEquality()
    {
        Assert.Equal(CreateSettings(), CreateSettings());
    }

    [Fact]
    public void Default_UsesExpectedValues()
    {
        var settings = PrintSettings.Default;

        Assert.Equal(PaperSize.A4, settings.PaperSize);
        Assert.Equal(ColorMode.BlackAndWhite, settings.ColorMode);
        Assert.Equal(SidesMode.SingleSided, settings.SidesMode);
        Assert.Equal(PageOrientation.Portrait, settings.Orientation);
        Assert.Equal(1, settings.Copies);
    }

    [Fact]
    public void Default_CreatesEqualSettingsWithDifferentReferences()
    {
        var first = PrintSettings.Default;
        var second = PrintSettings.Default;

        Assert.Equal(first, second);
        Assert.NotSame(first, second);
    }

    public static TheoryData<PrintSettings> DifferentSettings =>
        new()
        {
            new PrintSettings(PaperSize.A3, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait, 1),
            new PrintSettings(PaperSize.A4, ColorMode.Color, SidesMode.SingleSided, PageOrientation.Portrait, 1),
            new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.DoubleSided, PageOrientation.Portrait, 1),
            new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Landscape, 1),
            new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait, 2)
        };

    [Theory]
    [MemberData(nameof(DifferentSettings))]
    public void Equals_DiffersWhenEachIndividualPropertyChanges(PrintSettings differentSettings)
    {
        var baseSettings = CreateSettings();

        Assert.NotEqual(baseSettings, differentSettings);
    }

    [Fact]
    public void WithExpression_CreatesACopy()
    {
        var settings = CreateSettings();

        var copy = settings with { Copies = 2 };

        Assert.Equal(2, copy.Copies);
        Assert.Equal(settings.PaperSize, copy.PaperSize);
        Assert.Equal(settings.ColorMode, copy.ColorMode);
        Assert.Equal(settings.SidesMode, copy.SidesMode);
        Assert.Equal(settings.Orientation, copy.Orientation);
    }

    [Fact]
    public void Copy_PreservesValuesAndCreatesDifferentReference()
    {
        var settings = ColorSettings();

        var copy = settings.Copy();

        Assert.Equal(settings, copy);
        Assert.NotSame(settings, copy);
    }

    [Fact]
    public void WithExpression_FromCopy_DoesNotChangeOriginal()
    {
        var settings = CreateSettings();
        var copy = settings.Copy();

        var variant = copy with { Copies = 3 };

        Assert.Equal(1, settings.Copies);
        Assert.Equal(1, copy.Copies);
        Assert.Equal(3, variant.Copies);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000)]
    public void Constructor_RejectsInvalidCopies(int copies)
    {
        Assert.Throws<DomainRuleException>(
            () => new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait, copies));
    }

    [Theory]
    [InlineData((PaperSize)999, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait)]
    [InlineData(PaperSize.A4, (ColorMode)999, SidesMode.SingleSided, PageOrientation.Portrait)]
    [InlineData(PaperSize.A4, ColorMode.BlackAndWhite, (SidesMode)999, PageOrientation.Portrait)]
    [InlineData(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, (PageOrientation)999)]
    public void Constructor_RejectsUndefinedEnums(
        PaperSize paperSize,
        ColorMode colorMode,
        SidesMode sidesMode,
        PageOrientation orientation)
    {
        Assert.Throws<DomainRuleException>(
            () => new PrintSettings(paperSize, colorMode, sidesMode, orientation, 1));
    }

    private static PrintSettings CreateSettings()
    {
        return new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait, 1);
    }

    private static PrintSettings ColorSettings()
    {
        return new PrintSettings(PaperSize.A3, ColorMode.Color, SidesMode.DoubleSided, PageOrientation.Landscape, 2);
    }
}
