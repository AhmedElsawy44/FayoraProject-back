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
        string MainImageUrl,
        List<string> ImageUrls,
        List<int> IncludedItemIds,
        List<int>? ExcludedItemIds,
        List<PackageActivityResult> Activities,
        GeoPointResult MeetingPoint,
        GuideInfoResult GuideInfo,
        List<int> LocationIds
    );

    public record PackageActivityResult(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional
    );

    public record GeoPointResult(
        decimal Latitude,
        decimal Longitude
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