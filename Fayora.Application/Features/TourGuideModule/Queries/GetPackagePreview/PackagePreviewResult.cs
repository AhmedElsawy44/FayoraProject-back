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
        GeoPointResult MeetingPoint,
        GuideInfoResult GuideInfo,
        List<int> LocationIds
    );

    public record PackageActivityResult(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId
    );

    public record PackageNightResult(
    Guid Id,
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,
    Guid? PackageAccommodationId
);

    public record GeoPointResult(
        decimal Latitude,
        decimal Longitude
    );

    public record GuideInfoResult(
        string FirstName,
        string LastName,
        string? ProfileImageUrl,
        decimal AverageRating,
        int ReviewCount,
        int CompletedToursCount
    );
}