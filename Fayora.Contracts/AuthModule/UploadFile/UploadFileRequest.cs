using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Contracts.AuthModule.UploadFile;

public class UploadFileRequest
{
    [FromForm(Name = "Files")]
    public List<IFormFile> Files { get; set; } = new();
    [FromForm(Name = "Context")]
    public string Context { get; set; } = string.Empty;
}
