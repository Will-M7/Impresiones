namespace Impresiones.Application.Files;

/// <summary>
/// MIME is captured for a future validation step; Commit 07 validates only the file name and extension whitelist.
/// </summary>
public sealed record PrintableFileValidationRequest(
    string FileName,
    string? MimeType);
