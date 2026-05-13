using Fayora.Contracts.TourGuideModule.GetPackagePreview;

namespace Fayora.Contracts.TourGuideModule.GetPackageDetails
{
    public record PackageDetailsResponse(
        string Title,
        string Description,
        decimal AdultPrice,
        decimal ChildPrice,
        int DurationHours,
        string MainImageUrl,
        List<string> ImageUrls,
        List<int> IncludedItemIds,
        List<int>? ExcludedItemIds,
        List<PackageActivityDetailsResponse> Activities,
        GeoPointResponse MeetingPoint,
        GuideInfoResponse GuideInfo,
        string CancellationPolicy,
        string TransportType,
        string? GuestRequirements,
        string? ArrivalNote,
        List<PackageOccurrenceResponse> Occurrences
    );

    public record PackageActivityDetailsResponse(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional
    );

    public record PackageOccurrenceResponse(
        Guid Id,
        DateOnly Date,
        int AvailableSeats
    );
}
