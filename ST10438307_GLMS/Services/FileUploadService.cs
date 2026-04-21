// base file upload service - saves files

using Microsoft.AspNetCore.Components.Forms;
using ST10438307_GLMS.Decorators;

namespace ST10438307_GLMS.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _env;

    public FileUploadService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAsync(IBrowserFile file)
    {
        //File Saving
        //-------------------------------------------------------
        var fileName = $"{Guid.NewGuid()}_{file.Name}"; 
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

        await using var fs = new FileStream(uploadPath, FileMode.Create);
        await file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).CopyToAsync(fs);
        //-------------------------------------------------------

        return $"uploads/{fileName}"; //Path stored
    }
}