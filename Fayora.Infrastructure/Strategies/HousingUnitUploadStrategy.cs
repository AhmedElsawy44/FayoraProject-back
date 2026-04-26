using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Common.Models;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Infrastructure.Strategies;

public class HousingUnitUploadStrategy(IStorageService storageService, IDailyUploadTracker dailyUploadTracker, IClientContextProvider clientContextProvider)
    : BaseUploadStrategy(dailyUploadTracker, clientContextProvider)
{
    public override UploadContext Context => UploadContext.HousingUnit;
    protected override UploadLimits Limits => new(
        MaxFileCountPerRequest: 15,
        MaxFileSizeInBytes: 5 * 1024 * 1024,
        AllowedExtensions: [".jpg", ".jpeg", ".png", ".webp"],
        MaxFilesPerDay: 100
    );

    protected override async Task<Result<List<string>>> PerformUploadAsync(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var urls = new List<FileUrl>();
        foreach (var file in files)
        {
            var url = await storageService.SaveFileAsync(file, "housing-units", cancellationToken);
            urls.Add(FileUrl.Create(url).Value);
        }
        return urls.Select(v => v.Value).ToList();
    }
}