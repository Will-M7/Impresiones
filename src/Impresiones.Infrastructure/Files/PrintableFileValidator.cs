using Impresiones.Application.Files;
using Impresiones.Domain.Enums;

namespace Impresiones.Infrastructure.Files;

public sealed class PrintableFileValidator : IPrintableFileValidator
{
    private static readonly IReadOnlyDictionary<string, PrintableDocumentType> AllowedExtensions =
        new Dictionary<string, PrintableDocumentType>(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = PrintableDocumentType.Pdf,
            [".doc"] = PrintableDocumentType.Word,
            [".docx"] = PrintableDocumentType.Word,
            [".docm"] = PrintableDocumentType.Word,
            [".ppt"] = PrintableDocumentType.PowerPoint,
            [".pptx"] = PrintableDocumentType.PowerPoint,
            [".pptm"] = PrintableDocumentType.PowerPoint,
            [".jpg"] = PrintableDocumentType.Image,
            [".jpeg"] = PrintableDocumentType.Image,
            [".png"] = PrintableDocumentType.Image,
            [".webp"] = PrintableDocumentType.Image,
            [".bmp"] = PrintableDocumentType.Image
        };

    // MIME is preserved in the request contract for future validation, but Commit 07 only trusts the extension whitelist.
    public PrintableFileValidationResult Validate(PrintableFileValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return PrintableFileValidationResult.Invalid("File name is required.");
        }

        var extension = Path.GetExtension(request.FileName.Trim());
        if (string.IsNullOrWhiteSpace(extension))
        {
            return PrintableFileValidationResult.Invalid("File extension is required.");
        }

        if (!AllowedExtensions.TryGetValue(extension, out var documentType))
        {
            return PrintableFileValidationResult.Invalid("File extension is not allowed.");
        }

        return PrintableFileValidationResult.Valid(documentType);
    }
}
