using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Entities;

public sealed class PrintJob
{
    private readonly List<PrintDocument> documents = [];

    public PrintJob(string id, PhoneNumber phoneNumber, DateTimeOffset createdAt)
    {
        Id = EnsureRequired(id, nameof(id));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        CreatedAt = createdAt;
    }

    public string Id { get; }

    public PhoneNumber PhoneNumber { get; }

    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyCollection<PrintDocument> Documents => documents.AsReadOnly();

    public void AddDocument(PrintDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (documents.Any(existingDocument => existingDocument.Id == document.Id))
        {
            throw new DomainRuleException("A document with the same identifier already exists in the print job.");
        }

        documents.Add(document);
    }

    public PrintDocument? FindDocument(string documentId)
    {
        ArgumentNullException.ThrowIfNull(documentId);
        return documents.SingleOrDefault(document => document.Id == documentId);
    }

    public PrintDocument GetDocument(string documentId)
    {
        var document = FindDocument(documentId);
        return document ?? throw new DomainRuleException("Document was not found in the print job.");
    }

    public void UpdateDocumentSettings(string documentId, PrintSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        GetDocument(documentId).UpdateSettings(settings);
    }

    public void ApplySettingsToEditableDocuments(PrintSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        foreach (var document in documents.Where(document => document.CanUpdateSettings))
        {
            document.UpdateSettings(settings);
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
