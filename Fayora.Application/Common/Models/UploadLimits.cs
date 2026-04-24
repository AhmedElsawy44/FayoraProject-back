namespace Fayora.Application.Common.Models;

public record UploadLimits(
    int MaxFileCount,
    long MaxFileSizeInBytes,
    string[] AllowedExtensions);