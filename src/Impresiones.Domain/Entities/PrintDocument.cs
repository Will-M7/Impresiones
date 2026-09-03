using Impresiones.Domain.Enums;
using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Entities;

public sealed class PrintDocument
{
    public PrintDocument(
        string id,
        string storedFileName,
        PrintableDocumentType documentType,
        DateTimeOffset receivedAt)
        : this(id, storedFileName, documentType, receivedAt, PrintSettings.Default)
    {
    }

    public PrintDocument(
        string id,
        string storedFileName,
        PrintableDocumentType documentType,
        DateTimeOffset receivedAt,
        PrintSettings settings)
    {
        Id = EnsureRequired(id, nameof(id));
        StoredFileName = EnsureRequired(storedFileName, nameof(storedFileName));

        if (!Enum.IsDefined(documentType))
        {
            throw new DomainRuleException("Document type has an unsupported value.");
        }

        DocumentType = documentType;
        ReceivedAt = receivedAt;
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Status = PrintDocumentStatus.Pending;
    }

    public string Id { get; }

    public string StoredFileName { get; }

    public PrintableDocumentType DocumentType { get; }

    public DateTimeOffset ReceivedAt { get; }

    public PrintSettings Settings { get; private set; }

    public PrintDocumentStatus Status { get; private set; }

    public bool CanUpdateSettings => Status is PrintDocumentStatus.Pending or PrintDocumentStatus.Processing;

    public void UpdateSettings(PrintSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (!CanUpdateSettings)
        {
            throw new DomainRuleException("Printed or discriminated documents cannot be changed.");
        }

        Settings = settings;
    }

    public void StartProcessing()
    {
        EnsureTransitionTo(PrintDocumentStatus.Processing);
        Status = PrintDocumentStatus.Processing;
    }

    public void MarkAsPrinted()
    {
        EnsureTransitionTo(PrintDocumentStatus.Printed);
        Status = PrintDocumentStatus.Printed;
    }

    public void MarkAsDiscriminated()
    {
        EnsureTransitionTo(PrintDocumentStatus.Discriminated);
        Status = PrintDocumentStatus.Discriminated;
    }

    private void EnsureTransitionTo(PrintDocumentStatus targetStatus)
    {
        var isValidTransition = Status switch
        {
            PrintDocumentStatus.Pending => targetStatus is PrintDocumentStatus.Processing or PrintDocumentStatus.Discriminated,
            PrintDocumentStatus.Processing => targetStatus is PrintDocumentStatus.Printed or PrintDocumentStatus.Discriminated,
            _ => false
        };

        if (!isValidTransition)
        {
            throw new DomainRuleException($"Cannot change document status from {Status} to {targetStatus}.");
        }
    }

    private static string EnsureRequired(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException($"{parameterName} is required.");
        }

        return value;
    }
}
