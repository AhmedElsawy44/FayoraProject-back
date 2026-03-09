using Microsoft.AspNetCore.Http;

namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
}
