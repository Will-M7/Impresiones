using Impresiones.Application.Exceptions;
using Impresiones.Domain.Entities;

namespace Impresiones.Application.PrintJobs;

public sealed class ApplyPrintSettingsToAllDocumentsHandler
{
    private readonly IPrintJobRepository repository;

    public ApplyPrintSettingsToAllDocumentsHandler(IPrintJobRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ApplyPrintSettingsToAllDocumentsResult> HandleAsync(
        ApplyPrintSettingsToAllDocumentsCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var printJobId = EnsureRequired(command.PrintJobId, nameof(command.PrintJobId));
        var sourceDocumentId = EnsureRequired(command.SourceDocumentId, nameof(command.SourceDocumentId));

        var printJob = await repository.GetByIdAsync(printJobId, cancellationToken);
        if (printJob is null)
        {
            throw new ApplicationRuleException("Print job was not found.");
        }

        var sourceDocument = printJob.FindDocument(sourceDocumentId);
        if (sourceDocument is null)
        {
            throw new ApplicationRuleException("Source document was not found in the print job.");
        }

        var updatedDocuments = printJob.Documents.Count(document => document.CanUpdateSettings);
        var omittedDocuments = printJob.Documents.Count(document => !document.CanUpdateSettings);

        printJob.ApplySettingsToEditableDocuments(sourceDocument.Settings);

        await repository.SaveAsync(printJob, cancellationToken);

        return new ApplyPrintSettingsToAllDocumentsResult(updatedDocuments, omittedDocuments);
    }

    private static string EnsureRequired(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        return value;
    }
}
