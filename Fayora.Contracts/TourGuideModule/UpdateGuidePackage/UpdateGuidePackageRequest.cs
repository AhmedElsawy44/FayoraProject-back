using Fayora.Contracts.TourGuideModule.CreateGuidePackage;

namespace Fayora.Contracts.TourGuideModule.UpdateGuidePackage;

public record UpdateGuidePackageRequest(
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
