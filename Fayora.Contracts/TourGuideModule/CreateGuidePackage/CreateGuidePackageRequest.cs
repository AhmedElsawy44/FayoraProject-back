namespace Fayora.Contracts.TourGuideModule.CreateGuidePackage;

public record CreateGuidePackageRequest
(
    string Title,
    string Description,
    string TourType,
    int DurationHours,
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
    List<ActivityDto> Activities
);

public record ActivityDto(decimal Latitude, decimal Longitude, string Description, DateTimeOffset ActivityTime, bool IsOptional);
