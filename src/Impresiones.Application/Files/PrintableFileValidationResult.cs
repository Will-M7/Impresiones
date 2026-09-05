using Impresiones.Domain.Enums;

namespace Impresiones.Application.Files;

public sealed record PrintableFileValidationResult(
    bool IsValid,
    PrintableDocumentType? DocumentType,
    string? RejectionReason)
{
    public static PrintableFileValidationResult Valid(PrintableDocumentType documentType)
    {
        return new PrintableFileValidationResult(true, documentType, null);
    }

    public static PrintableFileValidationResult Invalid(string rejectionReason)
    {
        return new PrintableFileValidationResult(false, null, rejectionReason);
    }
}
