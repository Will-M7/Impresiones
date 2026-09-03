using Impresiones.Application.Exceptions;
using Impresiones.Application.PrintJobs;
using Impresiones.Domain.Entities;
using Impresiones.Domain.Enums;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Application.Tests.PrintJobs;

public class ApplyPrintSettingsToAllDocumentsHandlerTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 9, 3, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ReceivedAt = new(2026, 9, 3, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task HandleAsync_AppliesSourceSettingsToPendingDocuments()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var pending = CreateDocument("pending");
        job.AddDocument(source);
        job.AddDocument(pending);
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(source.Settings, pending.Settings);
    }

    [Fact]
    public async Task HandleAsync_AppliesSourceSettingsToProcessingDocuments()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var processing = CreateDocument("processing");
        processing.StartProcessing();
        job.AddDocument(source);
        job.AddDocument(processing);
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(source.Settings, processing.Settings);
    }

    [Fact]
    public async Task HandleAsync_DoesNotModifyPrintedDocuments()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var printed = CreateDocument("printed");
        printed.StartProcessing();
        printed.MarkAsPrinted();
        var originalSettings = printed.Settings;
        job.AddDocument(source);
        job.AddDocument(printed);
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(originalSettings, printed.Settings);
    }

    [Fact]
    public async Task HandleAsync_DoesNotModifyDiscriminatedDocuments()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var discriminated = CreateDocument("discriminated");
        discriminated.MarkAsDiscriminated();
        var originalSettings = discriminated.Settings;
        job.AddDocument(source);
        job.AddDocument(discriminated);
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(originalSettings, discriminated.Settings);
    }

    [Fact]
    public async Task HandleAsync_AppliesIndependentSettingsCopies()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var first = CreateDocument("first");
        var second = CreateDocument("second");
        job.AddDocument(source);
        job.AddDocument(first);
        job.AddDocument(second);
        var sourceSettings = source.Settings;
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(sourceSettings, source.Settings);
        Assert.Equal(sourceSettings, first.Settings);
        Assert.Equal(sourceSettings, second.Settings);
        Assert.NotSame(sourceSettings, source.Settings);
        Assert.NotSame(source.Settings, first.Settings);
        Assert.NotSame(first.Settings, second.Settings);
    }

    [Fact]
    public async Task HandleAsync_AllowsDocumentsToBeEditedIndividuallyAfterApplyingSettings()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var first = CreateDocument("first");
        var second = CreateDocument("second");
        job.AddDocument(source);
        job.AddDocument(first);
        job.AddDocument(second);
        var handler = CreateHandler(job);
        var individualSettings = new PrintSettings(PaperSize.A4, ColorMode.Color, SidesMode.SingleSided, PageOrientation.Portrait, 3);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));
        job.UpdateDocumentSettings("first", individualSettings);

        Assert.Equal(individualSettings, first.Settings);
        Assert.Equal(source.Settings, second.Settings);
        Assert.NotEqual(individualSettings, second.Settings);
    }

    [Fact]
    public async Task HandleAsync_SavesOnceWhenOperationSucceeds()
    {
        var job = CreateJobWithEditableDocuments();
        var repository = new FakePrintJobRepository(job);
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(1, repository.SaveCount);
        Assert.Same(job, repository.SavedPrintJob);
    }

    [Fact]
    public async Task HandleAsync_ReturnsUpdatedAndOmittedDocumentCounts()
    {
        var job = CreateJobWithMixedDocuments();
        var handler = CreateHandler(job);

        var result = await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        Assert.Equal(2, result.UpdatedDocuments);
        Assert.Equal(2, result.OmittedDocuments);
    }

    [Fact]
    public async Task HandleAsync_RejectsMissingPrintJobAndDoesNotSave()
    {
        var repository = new FakePrintJobRepository(null);
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);

        await Assert.ThrowsAsync<ApplicationRuleException>(
            () => handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("missing", "source")));

        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task HandleAsync_RejectsMissingSourceDocumentAndDoesNotSave()
    {
        var repository = new FakePrintJobRepository(CreateJobWithEditableDocuments());
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);

        await Assert.ThrowsAsync<ApplicationRuleException>(
            () => handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "missing")));

        Assert.Equal(0, repository.SaveCount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_RejectsMissingPrintJobIdentifier(string? printJobId)
    {
        var repository = new FakePrintJobRepository(CreateJobWithEditableDocuments());
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);
        var command = new ApplyPrintSettingsToAllDocumentsCommand(printJobId!, "source");

        await Assert.ThrowsAnyAsync<Exception>(() => handler.HandleAsync(command));

        Assert.Equal(0, repository.SaveCount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_RejectsMissingSourceDocumentIdentifier(string? sourceDocumentId)
    {
        var repository = new FakePrintJobRepository(CreateJobWithEditableDocuments());
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);
        var command = new ApplyPrintSettingsToAllDocumentsCommand("job-1", sourceDocumentId!);

        await Assert.ThrowsAnyAsync<Exception>(() => handler.HandleAsync(command));

        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task HandleAsync_PassesCancellationTokenToRepository()
    {
        var repository = new FakePrintJobRepository(CreateJobWithEditableDocuments());
        var handler = new ApplyPrintSettingsToAllDocumentsHandler(repository);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"), cancellationToken);

        Assert.Equal(cancellationToken, repository.GetByIdCancellationToken);
        Assert.Equal(cancellationToken, repository.SaveCancellationToken);
    }

    [Fact]
    public async Task HandleAsync_WithNoEditableDocumentsDoesNotChangeTerminalDocumentsAndReturnsZeroUpdated()
    {
        var job = CreateJob();
        var printed = CreateDocument("printed", ColorSettings());
        printed.StartProcessing();
        printed.MarkAsPrinted();
        var discriminated = CreateDocument("discriminated");
        discriminated.MarkAsDiscriminated();
        var originalPrintedSettings = printed.Settings;
        var originalDiscriminatedSettings = discriminated.Settings;
        job.AddDocument(printed);
        job.AddDocument(discriminated);
        var handler = CreateHandler(job);

        var result = await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "printed"));

        Assert.Equal(0, result.UpdatedDocuments);
        Assert.Equal(2, result.OmittedDocuments);
        Assert.Equal(originalPrintedSettings, printed.Settings);
        Assert.Equal(originalDiscriminatedSettings, discriminated.Settings);
    }

    [Fact]
    public async Task HandleAsync_DoesNotChangeDocumentStatus()
    {
        var job = CreateJobWithMixedDocuments();
        var statuses = job.Documents.ToDictionary(document => document.Id, document => document.Status);
        var handler = CreateHandler(job);

        await handler.HandleAsync(new ApplyPrintSettingsToAllDocumentsCommand("job-1", "source"));

        foreach (var document in job.Documents)
        {
            Assert.Equal(statuses[document.Id], document.Status);
        }
    }

    private static ApplyPrintSettingsToAllDocumentsHandler CreateHandler(PrintJob printJob)
    {
        return new ApplyPrintSettingsToAllDocumentsHandler(new FakePrintJobRepository(printJob));
    }

    private static PrintJob CreateJobWithEditableDocuments()
    {
        var job = CreateJob();
        job.AddDocument(CreateDocument("source", ColorSettings()));
        job.AddDocument(CreateDocument("pending"));
        return job;
    }

    private static PrintJob CreateJobWithMixedDocuments()
    {
        var job = CreateJob();
        var source = CreateDocument("source", ColorSettings());
        var processing = CreateDocument("processing");
        processing.StartProcessing();
        var printed = CreateDocument("printed");
        printed.StartProcessing();
        printed.MarkAsPrinted();
        var discriminated = CreateDocument("discriminated");
        discriminated.MarkAsDiscriminated();

        job.AddDocument(source);
        job.AddDocument(processing);
        job.AddDocument(printed);
        job.AddDocument(discriminated);

        return job;
    }

    private static PrintJob CreateJob()
    {
        return new PrintJob("job-1", new PhoneNumber("123456789"), CreatedAt);
    }

    private static PrintDocument CreateDocument(string id, PrintSettings? settings = null)
    {
        return new PrintDocument(id, $"{id}.pdf", PrintableDocumentType.Pdf, ReceivedAt, settings ?? PrintSettings.Default);
    }

    private static PrintSettings ColorSettings()
    {
        return new PrintSettings(PaperSize.A3, ColorMode.Color, SidesMode.DoubleSided, PageOrientation.Landscape, 2);
    }

    private sealed class FakePrintJobRepository : IPrintJobRepository
    {
        private readonly PrintJob? printJob;

        public FakePrintJobRepository(PrintJob? printJob)
        {
            this.printJob = printJob;
        }

        public int SaveCount { get; private set; }

        public PrintJob? SavedPrintJob { get; private set; }

        public CancellationToken GetByIdCancellationToken { get; private set; }

        public CancellationToken SaveCancellationToken { get; private set; }

        public Task<PrintJob?> GetByIdAsync(string printJobId, CancellationToken cancellationToken)
        {
            GetByIdCancellationToken = cancellationToken;
            return Task.FromResult(printJob);
        }

        public Task SaveAsync(PrintJob printJob, CancellationToken cancellationToken)
        {
            SaveCount++;
            SavedPrintJob = printJob;
            SaveCancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }
}
