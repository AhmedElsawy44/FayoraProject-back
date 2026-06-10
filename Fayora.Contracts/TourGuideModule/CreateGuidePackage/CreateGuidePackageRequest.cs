namespace Fayora.Contracts.TourGuideModule.CreateGuidePackage;

public record CreateGuidePackageRequest
(
    string Title,
    string Description,
    string TourType,
    int DurationHours,
    int NumOfDays,
    List<NightDto>? Nights,
    decimal Longitude,
    decimal Latitude,
    string TransportType,
    string? ArrivalNote,
    decimal AdultPrice,
    decimal ChildPrice,
    int MaxCapacity,
    List<int> IncludedIds,
    List<int> ExcludedIds,
    string MainImageUrl,
    string? VideoURL,
    List<string> ImageURLs,
    string CancellationPolicy,
    string? GuestRequirements,
    List<ActivityDto> Activities,
    HashSet<int> LocationIds
);

public record ActivityDto(decimal Latitude,
    decimal Longitude,
    string Description,
    TimeOnly ActivityTime,
    bool IsOptional, 
    int? LocationId);

public record NightDto(
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,
    PackageAccommodationDto? NewAccommodation
);

public record PackageAccommodationDto(
    string Name,
    string? Description,
    string Type,
    string MainImageUrl,
    List<string>? GalleryImages,
    decimal Latitude,
    decimal Longitude,
    TimeOnly CheckInTime,
    TimeOnly CheckOutTime,
    List<string>? Amenities
);