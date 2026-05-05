using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public class PackageImageUploadStrategy(
IStorageService storageService,
IDailyUploadTracker dailyTracker,
IClientContextProvider clientContextProvider)
: BaseUploadStrategy(dailyTracker, clientContextProvider)
{
    public override UploadContext Context => UploadContext.Package;

    protected override UploadLimits Limits => new(
        MaxFileCountPerRequest: 10,
        MaxFileSizeInBytes: 5 * 1024 * 1024,
        AllowedExtensions: [".jpg", ".jpeg", ".png", ".webp"],
        MaxFilesPerDay: 40
    );

    protected override async Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var urls = new List<FileUrl>();
        foreach (var file in files)
        {
            var url = await storageService.SaveFileAsync(file, "packages", cancellationToken);

            var fileUrlResult = FileUrl.Create(url);
            if (fileUrlResult.IsSuccess)
            {
                urls.Add(fileUrlResult.Value);
            }
        }
        return urls.Select(v => v.Value).ToList();
    }
}
