namespace Impresiones.Application.PrintJobs;

public sealed record ApplyPrintSettingsToAllDocumentsCommand(
    string PrintJobId,
    string SourceDocumentId);
