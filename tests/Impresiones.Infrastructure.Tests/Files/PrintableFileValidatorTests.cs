using Impresiones.Application.Files;
using Impresiones.Domain.Enums;
using Impresiones.Infrastructure.Files;

namespace Impresiones.Infrastructure.Tests.Files;

public class PrintableFileValidatorTests
{
    [Theory]
    [InlineData("documento.pdf", PrintableDocumentType.Pdf)]
    [InlineData("documento.doc", PrintableDocumentType.Word)]
    [InlineData("documento.docx", PrintableDocumentType.Word)]
    [InlineData("documento.docm", PrintableDocumentType.Word)]
    [InlineData("presentacion.ppt", PrintableDocumentType.PowerPoint)]
    [InlineData("presentacion.pptx", PrintableDocumentType.PowerPoint)]
    [InlineData("presentacion.pptm", PrintableDocumentType.PowerPoint)]
    [InlineData("imagen.jpg", PrintableDocumentType.Image)]
    [InlineData("imagen.jpeg", PrintableDocumentType.Image)]
    [InlineData("imagen.png", PrintableDocumentType.Image)]
    [InlineData("imagen.webp", PrintableDocumentType.Image)]
    [InlineData("imagen.bmp", PrintableDocumentType.Image)]
    public void Validate_AcceptsAllowedExtensions(string fileName, PrintableDocumentType expectedType)
    {
        var result = Validate(fileName, "application/octet-stream");

        Assert.True(result.IsValid);
        Assert.Equal(expectedType, result.DocumentType);
        Assert.Null(result.RejectionReason);
    }

    [Theory]
    [InlineData("DOCUMENTO.PDF", PrintableDocumentType.Pdf)]
    [InlineData("imagen.JPG", PrintableDocumentType.Image)]
    [InlineData("presentacion.PptX", PrintableDocumentType.PowerPoint)]
    public void Validate_IgnoresExtensionCasing(string fileName, PrintableDocumentType expectedType)
    {
        var result = Validate(fileName, null);

        Assert.True(result.IsValid);
        Assert.Equal(expectedType, result.DocumentType);
    }

    [Fact]
    public void Validate_AcceptsNamesWithMultipleDots()
    {
        var result = Validate("documento.final.v2.PDF", null);

        Assert.True(result.IsValid);
        Assert.Equal(PrintableDocumentType.Pdf, result.DocumentType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_RejectsMissingFileName(string? fileName)
    {
        var result = Validate(fileName!, null);

        Assert.False(result.IsValid);
        Assert.Null(result.DocumentType);
        Assert.Equal("File name is required.", result.RejectionReason);
    }

    [Fact]
    public void Validate_RejectsFileWithoutExtension()
    {
        var result = Validate("documento", null);

        Assert.False(result.IsValid);
        Assert.Equal("File extension is required.", result.RejectionReason);
    }

    [Theory]
    [InlineData("hoja.xls")]
    [InlineData("hoja.xlsx")]
    [InlineData("hoja.xlsm")]
    [InlineData("datos.csv")]
    [InlineData("nota.txt")]
    [InlineData("archivo.bin")]
    [InlineData("programa.exe")]
    [InlineData("audio.mp3")]
    [InlineData("audio.wav")]
    [InlineData("video.mp4")]
    [InlineData("video.mov")]
    [InlineData("desconocido.xyz")]
    public void Validate_RejectsDisallowedExtensions(string fileName)
    {
        var result = Validate(fileName, null);

        Assert.False(result.IsValid);
        Assert.Equal("File extension is not allowed.", result.RejectionReason);
    }

    [Fact]
    public void Validate_RejectsDoubleExtensionsByLastExtension()
    {
        var result = Validate("archivo.pdf.exe", "application/pdf");

        Assert.False(result.IsValid);
        Assert.Equal("File extension is not allowed.", result.RejectionReason);
    }

    [Fact]
    public void Validate_DoesNotUseDeclaredMimeAsValidityProof()
    {
        var result = Validate("documento.pdf", "video/mp4");

        Assert.True(result.IsValid);
        Assert.Equal(PrintableDocumentType.Pdf, result.DocumentType);
    }

    [Fact]
    public void Validate_RejectsNullRequest()
    {
        var validator = new PrintableFileValidator();

        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!));
    }

    [Fact]
    public void Validate_IsPureAndDoesNotRequireExistingFile()
    {
        var result = Validate(@"Z:\ruta\inexistente\documento.pdf", null);

        Assert.True(result.IsValid);
        Assert.Equal(PrintableDocumentType.Pdf, result.DocumentType);
    }

    private static PrintableFileValidationResult Validate(string fileName, string? mimeType)
    {
        var validator = new PrintableFileValidator();
        return validator.Validate(new PrintableFileValidationRequest(fileName, mimeType));
    }
}
