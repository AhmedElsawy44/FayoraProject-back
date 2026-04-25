using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Domain.Common.Results;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public abstract class BaseUploadStrategy(
        IDailyUploadTracker dailyTracker,
        IClientContextProvider clientContextProvider) : IUploadStrategy
{
    public abstract UploadContext Context { get; }
    protected abstract UploadLimits Limits { get; }

    public async Task<Result<List<string>>> UploadFilesAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (files.Count > Limits.MaxFileCountPerRequest)
            return Error.Validation("Upload.TooManyFiles", $"Max files per request is {Limits.MaxFileCountPerRequest}.");

        foreach (var file in files)
        {
            if (file.Length > Limits.MaxFileSizeInBytes)
                return Error.Validation("Upload.FileTooLarge", "File is too large.");
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Limits.AllowedExtensions.Contains(extension))
                return Error.Validation("Upload.InvalidFileType", $"File type {extension} is not allowed.");
        }

        var userId = clientContextProvider.GetContext().UserId;

        var todayCount = await dailyTracker.GetTodayUploadCountAsync(userId, Context, cancellationToken);

        if (todayCount + files.Count > Limits.MaxFilesPerDay)
        {
            return Error.Validation("Upload.DailyLimitExceeded",
                $"You have exceeded your daily limit for {Context}. You can upload {Limits.MaxFilesPerDay - todayCount} more files today.");
        }

        var uploadResult = await PerformUploadAsync(files, cancellationToken);

        if (uploadResult.IsSuccess)
        {
            await dailyTracker.IncrementUploadCountAsync(userId, Context, files.Count, cancellationToken);
        }

        return uploadResult;
    }

    protected abstract Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken);
}