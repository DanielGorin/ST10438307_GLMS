// decorator pattern interface

using Microsoft.AspNetCore.Components.Forms;

namespace ST10438307_GLMS.Decorators;

public interface IFileUploadService
{
    Task<string> UploadAsync(IBrowserFile file);
}