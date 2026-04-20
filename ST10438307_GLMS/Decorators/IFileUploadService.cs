using Microsoft.AspNetCore.Components.Forms;

namespace ST10438307_GLMS.Decorators;

//Defines file uplaods

public interface IFileUploadService
{
    Task<string> UploadAsync(IBrowserFile file);
}