using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fayora.Infrastructure.Services.AuthServices;

public class LocalFileService(IWebHostEnvironment env, ILogger<LocalFileService> logger) : IFileStorageService
{
    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("The file cannot be null or empty.", nameof(file));
        }

        if (string.IsNullOrWhiteSpace(folderName))
        {
            throw new ArgumentException("Folder name cannot be null or empty.", nameof(folderName));
        }

        var directoryPath = Path.Combine(env.WebRootPath, folderName);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(directoryPath, fileName);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "IO Error (file in use, etc.) while saving file to {FullPath}", fullPath);
            throw new InvalidOperationException("An input/output error occurred while saving the file.", ex);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An unexpected critical error occurred while saving the file to {FullPath}", fullPath);
            throw;
        }

        return fullPath;
    }
}