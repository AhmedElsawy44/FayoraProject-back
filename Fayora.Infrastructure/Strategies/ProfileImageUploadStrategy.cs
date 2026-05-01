using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public class ProfileImageUploadStrategy(IStorageService storageService, IDailyUploadTracker dailyUploadTracker, IClientContextProvider clientContextProvider)
    : BaseUploadStrategy(dailyUploadTracker, clientContextProvider)
{
    public override UploadContext Context => UploadContext.Profile;

    protected override UploadLimits Limits => new(
        MaxFileCountPerRequest: 1,
        MaxFileSizeInBytes: 2 * 1024 * 1024,
        AllowedExtensions: [".jpg", ".jpeg", ".png"],
        MaxFilesPerDay: 5
    );

    protected override async Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var url = await storageService.SaveFileAsync(files.First(), "profiles", cancellationToken);
        var fileUrlResult = FileUrl.Create(url);
        if (fileUrlResult.IsError) return fileUrlResult.Errors;

        return new List<string> { fileUrlResult.Value.Value };
    }
}