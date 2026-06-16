namespace Fayora.Contracts.AdminModule.UpdateLocation;

public record LocationPackageDto(
    Guid PackageId,
    string Title,
    Guid ProviderId,
    string ProviderName,
    decimal AdultPrice,
    decimal ChildPrice,
    string Status,
    string MainImageUrl
);

public record GetDetailedLocationResponse(
    int Id,
    string Name,
    string? Description,
    decimal Rating,
    decimal Latitude,
    decimal Longitude,
    string Category,
    string MainImageUrl,
    List<string> ImageUrls,
    List<LocationPackageDto> AssociatedPackages
);
