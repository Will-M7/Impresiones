namespace Impresiones.Application.PrintJobs;

public sealed record ApplyPrintSettingsToAllDocumentsResult(
    int UpdatedDocuments,
    int OmittedDocuments);
