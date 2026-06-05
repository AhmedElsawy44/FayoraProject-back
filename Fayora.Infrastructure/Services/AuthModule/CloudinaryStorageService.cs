using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Fayora.Infrastructure.Services.AuthModule;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken)
    {
        if (file.Length <= 0) return string.Empty;

        using var stream = file.OpenReadStream();

        var isImage = file.ContentType.StartsWith("image/");
        var isVideo = file.ContentType.StartsWith("video/") || 
                      new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" }.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"Fayora/{folder}",
            PublicId = Guid.NewGuid().ToString()
        };

        if (isImage)
        {
            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"Fayora/{folder}",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var imageResult = await _cloudinary.UploadAsync(imageParams, cancellationToken);
            return imageResult.SecureUrl.ToString();
        }
        else if (isVideo)
        {
            var videoParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"Fayora/{folder}"
            };

            var videoResult = await _cloudinary.UploadAsync(videoParams, cancellationToken);
            return videoResult.SecureUrl.ToString();
        }

        var result = await _cloudinary.UploadAsync(uploadParams, "raw");
        return result.SecureUrl.ToString();
    }
}
