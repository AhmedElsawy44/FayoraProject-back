namespace Fayora.Infrastructure.Services.SharedModule;

using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class LocalFileService(
    IWebHostEnvironment env,
    ILogger<LocalFileService> logger) : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
    {
        // Validate
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new ArgumentException($"File type '{ext}' is not allowed.");

        if (fileStream.Length > MaxFileSize)
            throw new ArgumentException("File exceeds 10MB limit.");

        // Create folder
        var directoryPath = Path.Combine(env.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(directoryPath);

        // Save file
        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(directoryPath, uniqueFileName);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(stream);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "IO Error while saving file to {FullPath}", fullPath);
            throw new InvalidOperationException("An error occurred while saving the file.", ex);
        }


        return $"/uploads/{folderName}/{uniqueFileName}";
    }
}