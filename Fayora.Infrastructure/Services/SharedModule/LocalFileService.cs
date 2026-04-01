namespace Fayora.Infrastructure.Services.SharedModule;

using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class LocalFileService(
    IWebHostEnvironment env,
    ILogger<LocalFileService> logger) : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new ArgumentException($"File type '{ext}' is not allowed.");

        if (fileStream.Length > MaxFileSize)
            throw new ArgumentException("File exceeds 10MB limit.");


        var directoryPath = Path.Combine(env.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(directoryPath);

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

    public Task DeleteFileAsync(string mediaURL, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(mediaURL))
        {
            logger.LogWarning("DeleteFileAsync was called with an empty URL.");
            return Task.CompletedTask;
        }

        try
        {
            var decodedUrl = Uri.UnescapeDataString(mediaURL);


            var relativePath = decodedUrl.TrimStart('/', '\\');

            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.Combine(env.WebRootPath, relativePath);

            var physicalPath = Path.GetFullPath(fullPath);
            if (!physicalPath.StartsWith(env.WebRootPath, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Security Alert: Path traversal attempt detected. URL: {MediaUrl}", mediaURL);
                throw new UnauthorizedAccessException("Invalid file path.");
            }

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                logger.LogInformation("File successfully deleted: {PhysicalPath}", physicalPath);
            }
            else
            {
                logger.LogWarning("File not found, could not delete: {PhysicalPath}", physicalPath);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while deleting file for URL: {MediaUrl}", mediaURL);
            throw new InvalidOperationException("An error occurred while deleting the file.", ex);
        }

        return Task.CompletedTask;
    }
}