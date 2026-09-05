namespace Impresiones.Application.Files;

public interface IPrintableFileValidator
{
    PrintableFileValidationResult Validate(PrintableFileValidationRequest request);
}
