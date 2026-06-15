using Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails;
using Fayora.Contracts.TourGuideModule.GetPackageDetails;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackagePreview
{
    public record PackagePreviewResult(
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
        List<PackageActivityResult> Activities,
        List<PackageNightResult> Nights,
        List<PackageMeetingPointResult> MeetingPoints,
        GuideInfoResult GuideInfo,
        List<int> LocationIds,
        string TourType,
        string TransportType,
        string CancellationPolicy,
        int MaxCapacity,
        string? ArrivalNote,
        string? VideoURL,
        string? GuestRequirements,
        List<OptionalActivityResponse>? OptionalActivities
    );

    public record PackageActivityResult(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId,
        decimal? Latitude,
        decimal? Longitude
    );

    public record PackageNightResult(
    Guid Id,
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,
    Guid? PackageAccommodationId
);



    public record GuideInfoResult(
        Guid GuideUserId,
        string FirstName,
        string LastName,
        string? ProfileImageUrl,
        decimal AverageRating,
        int ReviewCount,
        int CompletedToursCount
    );
}