namespace Fayora.Contracts.TourGuideModule.CreateGuidePackage;

public record CreateGuidePackageRequest
(
    string Title,
    string Description,
    string TourType,
    int DurationHours,
    int NumOfDays,
    List<NightDto>? Nights,
    List<MeetingPointDto> MeetingPoints,
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
    HashSet<int> LocationIds,
    List<OptionalActivityDto>? OptionalActivities,
    bool HasGroupDiscount = false,
    int? GroupDiscountMinPeople = null,
    decimal? GroupDiscountPercent = null
);

public record OptionalActivityDto(
    string Description,
    decimal AdditionalPrice,
    string ImageUrl
);

public record MeetingPointDto(
    string MeetingPointName,
    decimal Latitude,
    decimal Longitude,
    TimeOnly Time,
    decimal Price = 0,
    string? Description = null
);

public record ActivityDto(decimal? Latitude,
    decimal? Longitude,
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
    List<string>? Amenities,
    List<string>? Meals
);