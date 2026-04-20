using Microsoft.AspNetCore.Components.Forms;

namespace ST10438307_GLMS.Decorators;

// Validates PDFs
public class PdfValidationDecorator : IFileUploadService
{
    private readonly IFileUploadService _inner;

    public PdfValidationDecorator(IFileUploadService inner)
    {
        _inner = inner;
    }

    public async Task<string> UploadAsync(IBrowserFile file)
    {
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (extension != ".pdf")
        {
            throw new InvalidOperationException("Only PDF files are allowed. Please upload a .pdf file.");
        }

        return await _inner.UploadAsync(file);
    }
}