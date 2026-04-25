namespace Fayora.Application.Common.Models;

public record UploadLimits(
    int MaxFileCountPerRequest,
    long MaxFileSizeInBytes,
    string[] AllowedExtensions,
    int MaxFilesPerDay);