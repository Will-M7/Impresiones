using Impresiones.Domain.Entities;
using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Tests.Entities;

public class PrintJobTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 8, 31, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ReceivedAt = new(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_CreatesJobWithPhoneNumberDateAndEmptyCollection()
    {
        var phoneNumber = new PhoneNumber("123456789");
        var job = new PrintJob("job-1", phoneNumber, CreatedAt);

        Assert.Equal("job-1", job.Id);
        Assert.Equal(phoneNumber, job.PhoneNumber);
        Assert.Equal(CreatedAt, job.CreatedAt);
        Assert.Empty(job.Documents);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsEmptyIdentifier(string id)
    {
        Assert.Throws<DomainRuleException>(() => new PrintJob(id, new PhoneNumber("123456789"), CreatedAt));
    }

    [Fact]
    public void Constructor_RejectsNullIdentifier()
    {
        Assert.Throws<ArgumentNullException>(() => new PrintJob(null!, new PhoneNumber("123456789"), CreatedAt));
    }

    [Fact]
    public void Constructor_RejectsNullPhoneNumber()
    {
        Assert.Throws<ArgumentNullException>(() => new PrintJob("job-1", null!, CreatedAt));
    }

    [Fact]
    public void AddDocument_AddsDocuments()
    {
        var job = CreateJob();
        var document = CreateDocument("doc-1");

        job.AddDocument(document);

        Assert.Single(job.Documents);
        Assert.Same(document, job.Documents.Single());
    }

    [Fact]
    public void AddDocument_RejectsNullDocument()
    {
        var job = CreateJob();

        Assert.Throws<ArgumentNullException>(() => job.AddDocument(null!));
    }

    [Fact]
    public void AddDocument_RejectsDuplicateIdentifiers()
    {
        var job = CreateJob();
        job.AddDocument(CreateDocument("doc-1"));

        Assert.Throws<DomainRuleException>(() => job.AddDocument(CreateDocument("doc-1")));
    }

    [Fact]
    public void Documents_DoesNotAllowDirectModificationOfInternalCollection()
    {
        var job = CreateJob();
        job.AddDocument(CreateDocument("doc-1"));

        var exposedDocuments = Assert.IsAssignableFrom<ICollection<PrintDocument>>(job.Documents);

        Assert.Throws<NotSupportedException>(() => exposedDocuments.Add(CreateDocument("doc-2")));
        Assert.Single(job.Documents);
    }

    [Fact]
    public void FindDocument_ReturnsExistingDocument()
    {
        var job = CreateJob();
        var document = CreateDocument("doc-1");
        job.AddDocument(document);

        var found = job.FindDocument("doc-1");

        Assert.Same(document, found);
    }

    [Fact]
    public void FindDocument_ReturnsNullForMissingDocument()
    {
        var job = CreateJob();

        Assert.Null(job.FindDocument("missing"));
    }

    [Fact]
    public void GetDocument_RejectsMissingDocument()
    {
        var job = CreateJob();

        Assert.Throws<DomainRuleException>(() => job.GetDocument("missing"));
    }

    [Fact]
    public void UpdateDocumentSettings_UpdatesSpecificDocument()
    {
        var job = CreateJob();
        var first = CreateDocument("doc-1");
        var second = CreateDocument("doc-2");
        job.AddDocument(first);
        job.AddDocument(second);
        var settings = ColorSettings();

        job.UpdateDocumentSettings("doc-1", settings);

        Assert.Equal(settings, first.Settings);
        Assert.NotEqual(settings, second.Settings);
    }

    [Fact]
    public void UpdateDocumentSettings_RejectsMissingDocument()
    {
        var job = CreateJob();

        Assert.Throws<DomainRuleException>(() => job.UpdateDocumentSettings("missing", ColorSettings()));
    }

    [Fact]
    public void ApplySettingsToEditableDocuments_UpdatesPendingAndProcessingDocuments()
    {
        var job = CreateJob();
        var pending = CreateDocument("doc-1");
        var processing = CreateDocument("doc-2");
        processing.StartProcessing();
        job.AddDocument(pending);
        job.AddDocument(processing);
        var settings = ColorSettings();

        job.ApplySettingsToEditableDocuments(settings);

        Assert.Equal(settings, pending.Settings);
        Assert.Equal(settings, processing.Settings);
    }

    [Fact]
    public void ApplySettingsToEditableDocuments_DoesNotModifyPrintedOrDiscriminatedDocuments()
    {
        var job = CreateJob();
        var printed = CreateDocument("doc-1");
        printed.StartProcessing();
        printed.MarkAsPrinted();
        var discriminated = CreateDocument("doc-2");
        discriminated.MarkAsDiscriminated();
        var originalPrintedSettings = printed.Settings;
        var originalDiscriminatedSettings = discriminated.Settings;
        job.AddDocument(printed);
        job.AddDocument(discriminated);

        job.ApplySettingsToEditableDocuments(ColorSettings());

        Assert.Equal(originalPrintedSettings, printed.Settings);
        Assert.Equal(originalDiscriminatedSettings, discriminated.Settings);
    }

    [Fact]
    public void ApplySettingsToEditableDocuments_AllowsIndividualChangesAfterCopy()
    {
        var job = CreateJob();
        var first = CreateDocument("doc-1");
        var second = CreateDocument("doc-2");
        job.AddDocument(first);
        job.AddDocument(second);
        var copiedSettings = ColorSettings();
        var individualSettings = new PrintSettings(PaperSize.A4, ColorMode.Color, SidesMode.SingleSided, PageOrientation.Portrait, 3);

        job.ApplySettingsToEditableDocuments(copiedSettings);
        job.UpdateDocumentSettings("doc-1", individualSettings);

        Assert.Equal(individualSettings, first.Settings);
        Assert.Equal(copiedSettings, second.Settings);
    }

    [Fact]
    public void ApplySettingsToEditableDocuments_DoesNotFailWhenJobIsEmpty()
    {
        var job = CreateJob();

        job.ApplySettingsToEditableDocuments(ColorSettings());

        Assert.Empty(job.Documents);
    }

    [Fact]
    public void ApplySettingsToEditableDocuments_RejectsNullSettings()
    {
        var job = CreateJob();

        Assert.Throws<ArgumentNullException>(() => job.ApplySettingsToEditableDocuments(null!));
    }

    [Fact]
    public void UpdateDocumentSettings_RejectsNullSettings()
    {
        var job = CreateJob();
        job.AddDocument(CreateDocument("doc-1"));

        Assert.Throws<ArgumentNullException>(() => job.UpdateDocumentSettings("doc-1", null!));
    }

    [Fact]
    public void Jobs_DoNotShareDocuments()
    {
        var firstJob = CreateJob("job-1", "123456789");
        var secondJob = CreateJob("job-2", "987654321");
        firstJob.AddDocument(CreateDocument("doc-1"));
        secondJob.AddDocument(CreateDocument("doc-2"));

        Assert.NotSame(firstJob.Documents.Single(), secondJob.Documents.Single());
        Assert.Null(firstJob.FindDocument("doc-2"));
        Assert.Null(secondJob.FindDocument("doc-1"));
    }

    private static PrintJob CreateJob(string id = "job-1", string phoneNumber = "123456789")
    {
        return new PrintJob(id, new PhoneNumber(phoneNumber), CreatedAt);
    }

    private static PrintDocument CreateDocument(string id)
    {
        return new PrintDocument(id, $"{id}.pdf", PrintableDocumentType.Pdf, ReceivedAt, DefaultSettings());
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
