using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public abstract class BaseUploadStrategy(IStorageService storageService) : IUploadStrategy
{
    public abstract UploadContext Context { get; }
    protected abstract UploadLimits Limits { get; }

    public async Task<Result<List<string>>> UploadFilesAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        // 1. Validation: File Count
        if (files.Count > Limits.MaxFileCount)
            return Error.Validation("Upload.TooManyFiles", $"Maximum allowed files for {Context} is {Limits.MaxFileCount}. You sent {files.Count}.");

        // 2. Validation: File Size & Extensions
        foreach (var file in files)
        {
            if (file.Length > Limits.MaxFileSizeInBytes)
                return Error.Validation("Upload.FileTooLarge", $"File '{file.FileName}' exceeds the maximum allowed size of {Limits.MaxFileSizeInBytes / (1024 * 1024)}MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Limits.AllowedExtensions.Contains(extension))
                return Error.Validation("Upload.InvalidExtension", $"File '{file.FileName}' has an invalid extension. Allowed: {string.Join(", ", Limits.AllowedExtensions)}");
        }

        return await PerformUploadAsync(files, cancellationToken);
    }

    protected abstract Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken);
}
