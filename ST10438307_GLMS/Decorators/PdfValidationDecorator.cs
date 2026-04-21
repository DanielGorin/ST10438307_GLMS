// decorator - wraps FileUpload service and rvalidates

using Microsoft.AspNetCore.Components.Forms;

namespace ST10438307_GLMS.Decorators;

public class PdfValidationDecorator : IFileUploadService
{
    private readonly IFileUploadService _inner; // the service being wrapped

    public PdfValidationDecorator(IFileUploadService inner)
    {
        _inner = inner;
    }

    public async Task<string> UploadAsync(IBrowserFile file)
    {
        //Validation - check extension
        //-------------------------------------------------------
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (extension != ".pdf")
            throw new InvalidOperationException("only pdf files are allowed.");
        //-------------------------------------------------------

        return await _inner.UploadAsync(file); // passed validation
    }
}