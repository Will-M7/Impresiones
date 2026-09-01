using Impresiones.Domain.Entities;
using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Tests.Entities;

public class PrintDocumentTests
{
    private static readonly DateTimeOffset ReceivedAt = new(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_CreatesDocumentWithPendingStatus()
    {
        var document = CreateDocument();

        Assert.Equal("doc-1", document.Id);
        Assert.Equal("documento.pdf", document.StoredFileName);
        Assert.Equal(PrintableDocumentType.Pdf, document.DocumentType);
        Assert.Equal(ReceivedAt, document.ReceivedAt);
        Assert.Equal(PrintDocumentStatus.Pending, document.Status);
        Assert.Equal(DefaultSettings(), document.Settings);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsEmptyIdentifier(string id)
    {
        Assert.Throws<DomainRuleException>(() => CreateDocument(id: id));
    }

    [Fact]
    public void Constructor_RejectsNullIdentifier()
    {
        Assert.Throws<ArgumentNullException>(() => CreateDocument(id: null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsEmptyFileName(string fileName)
    {
        Assert.Throws<DomainRuleException>(() => CreateDocument(fileName: fileName));
    }

    [Fact]
    public void Constructor_RejectsNullFileName()
    {
        Assert.Throws<ArgumentNullException>(() => CreateDocument(fileName: null!));
    }

    [Fact]
    public void Constructor_RejectsNullSettings()
    {
        Assert.Throws<ArgumentNullException>(
            () => new PrintDocument("doc-1", "documento.pdf", PrintableDocumentType.Pdf, ReceivedAt, null!));
    }

    [Fact]
    public void Constructor_RejectsUndefinedDocumentType()
    {
        Assert.Throws<DomainRuleException>(() => CreateDocument(documentType: (PrintableDocumentType)999));
    }

    [Fact]
    public void UpdateSettings_AllowsChangesInPending()
    {
        var document = CreateDocument();
        var settings = ColorSettings();

        document.UpdateSettings(settings);

        Assert.Equal(settings, document.Settings);
    }

    [Fact]
    public void StartProcessing_AllowsPendingToProcessing()
    {
        var document = CreateDocument();

        document.StartProcessing();

        Assert.Equal(PrintDocumentStatus.Processing, document.Status);
    }

    [Fact]
    public void UpdateSettings_AllowsChangesInProcessing()
    {
        var document = CreateDocument();
        document.StartProcessing();
        var settings = ColorSettings();

        document.UpdateSettings(settings);

        Assert.Equal(settings, document.Settings);
    }

    [Fact]
    public void MarkAsPrinted_AllowsProcessingToPrinted()
    {
        var document = CreateDocument();
        document.StartProcessing();

        document.MarkAsPrinted();

        Assert.Equal(PrintDocumentStatus.Printed, document.Status);
    }

    [Fact]
    public void MarkAsDiscriminated_AllowsPendingToDiscriminated()
    {
        var document = CreateDocument();

        document.MarkAsDiscriminated();

        Assert.Equal(PrintDocumentStatus.Discriminated, document.Status);
    }

    [Fact]
    public void MarkAsDiscriminated_AllowsProcessingToDiscriminated()
    {
        var document = CreateDocument();
        document.StartProcessing();

        document.MarkAsDiscriminated();

        Assert.Equal(PrintDocumentStatus.Discriminated, document.Status);
    }

    [Fact]
    public void MarkAsPrinted_RejectsPendingToPrinted()
    {
        var document = CreateDocument();

        Assert.Throws<DomainRuleException>(document.MarkAsPrinted);
    }

    [Fact]
    public void StartProcessing_RejectsProcessingToProcessing()
    {
        var document = CreateDocument();
        document.StartProcessing();

        Assert.Throws<DomainRuleException>(document.StartProcessing);
    }

    [Fact]
    public void UpdateSettings_RejectsChangesFromPrinted()
    {
        var document = CreateDocument();
        document.StartProcessing();
        document.MarkAsPrinted();

        Assert.Throws<DomainRuleException>(() => document.UpdateSettings(ColorSettings()));
    }

    [Fact]
    public void UpdateSettings_RejectsChangesFromDiscriminated()
    {
        var document = CreateDocument();
        document.MarkAsDiscriminated();

        Assert.Throws<DomainRuleException>(() => document.UpdateSettings(ColorSettings()));
    }

    [Fact]
    public void TerminalStates_RejectFurtherTransitions()
    {
        var printed = CreateDocument();
        printed.StartProcessing();
        printed.MarkAsPrinted();

        var discriminated = CreateDocument(id: "doc-2");
        discriminated.MarkAsDiscriminated();

        Assert.Throws<DomainRuleException>(printed.MarkAsPrinted);
        Assert.Throws<DomainRuleException>(printed.MarkAsDiscriminated);
        Assert.Throws<DomainRuleException>(discriminated.MarkAsPrinted);
        Assert.Throws<DomainRuleException>(discriminated.MarkAsDiscriminated);
    }

    private static PrintDocument CreateDocument(
        string id = "doc-1",
        string fileName = "documento.pdf",
        PrintableDocumentType documentType = PrintableDocumentType.Pdf,
        PrintSettings? settings = null)
    {
        return new PrintDocument(id, fileName, documentType, ReceivedAt, settings ?? DefaultSettings());
    }

    private static PrintSettings DefaultSettings()
    {
        return new PrintSettings(PaperSize.A4, ColorMode.BlackAndWhite, SidesMode.SingleSided, PageOrientation.Portrait, 1);
    }

    private static PrintSettings ColorSettings()
    {
        return new PrintSettings(PaperSize.A3, ColorMode.Color, SidesMode.DoubleSided, PageOrientation.Landscape, 2);
    }
}
