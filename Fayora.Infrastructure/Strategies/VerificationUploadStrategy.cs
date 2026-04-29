using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Application.Common.Strategies;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public class VerificationUploadStrategy(
    IStorageService storageService,
    IDailyUploadTracker dailyTracker,
    IClientContextProvider clientContextProvider)
    : BaseUploadStrategy(dailyTracker, clientContextProvider)
{
    public override UploadContext Context => UploadContext.Verification;

    protected override UploadLimits Limits => new(
        MaxFileCountPerRequest: 2,
        MaxFileSizeInBytes: 3 * 1024 * 1024,
        AllowedExtensions: [".jpg", ".jpeg", ".png", ".pdf"],
        MaxFilesPerDay: 5
    );

    protected override async Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var urls = new List<FileUrl>();
        foreach (var file in files)
        {
            var url = await storageService.SaveFileAsync(file, "verifications", cancellationToken);

            var fileUrlResult = FileUrl.Create(url);
            if (fileUrlResult.IsSuccess)
            {
                urls.Add(fileUrlResult.Value);
            }
        }
        return urls.Select(u => u.Value).ToList();
    }
}
