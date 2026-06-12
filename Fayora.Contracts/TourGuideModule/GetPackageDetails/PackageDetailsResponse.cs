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
        GeoPointResponse MeetingPoint,
        GuideInfoResponse GuideInfo,
        string CancellationPolicy,
        string TransportType,
        string? GuestRequirements,
        string? ArrivalNote,
        List<int> LocationIds,
        List<PackageOccurrenceResponse> Occurrences
    );

    public record PackageActivityDetailsResponse(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId
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
