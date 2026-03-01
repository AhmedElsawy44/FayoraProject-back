using Microsoft.AspNetCore.Http;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
}
