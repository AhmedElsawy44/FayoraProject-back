using Fayora.Contracts.TourGuideModule.GetPackagePreview;

namespace Fayora.Contracts.TourGuideModule.GetPackageDetails
{
    public record PackageDetailsResponse(
        string Title,
        string Description,
        decimal AdultPrice,
        decimal ChildPrice,
        decimal DiscountedAdultPrice,
        decimal DiscountedChildPrice,
        int DurationHours,
        int NumOfDays,
        int NumOfNights,
        string MainImageUrl,
        List<string> ImageUrls,
        List<int> IncludedItemIds,
        List<int>? ExcludedItemIds,
        List<PackageActivityDetailsResponse> Activities,
        List<PackageNightDetailsResponse> Nights,
        List<PackageMeetingPointResponse> MeetingPoints,
        GuideInfoResponse GuideInfo,
        string CancellationPolicy,
        string TransportType,
        string? GuestRequirements,
        string? ArrivalNote,
        List<int> LocationIds,
        List<PackageOccurrenceResponse> Occurrences,
        List<OptionalActivityResponse>? OptionalActivities,
        string TourType,
        int MaxCapacity,
        string? VideoURL,
        bool HasGroupDiscount,
        int? GroupDiscountMinPeople,
        decimal? GroupDiscountPercent
    );

    public record OptionalActivityResponse(
        Guid Id,
        string Description,
        decimal AdditionalPrice,
        string ImageUrl
    );

    public record PackageMeetingPointResponse(
        Guid Id,
        string? MeetingPointName,
        decimal Latitude,
        decimal Longitude,
        TimeOnly Time,
        decimal Price,
        string? Description
    );

    public record PackageActivityDetailsResponse(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId,
        decimal? Latitude,
        decimal? Longitude
    );

    public record PackageNightDetailsResponse(
    Guid Id,
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,
    Guid? PackageAccommodationId
    );

    public record PackageOccurrenceResponse(
        Guid Id,
        DateOnly Date,
        int AvailableSeats
    );
}
