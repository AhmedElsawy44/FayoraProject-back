namespace Fayora.Contracts.TourGuideModule.GetPackagePreview
{
    public record PackagePreviewResponse(
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
        List<PackageActivityResponse> Activities,
        List<int> LocationIds,
        GeoPointResponse MeetingPoint,
        GuideInfoResponse GuideInfo
    );

    public record PackageActivityResponse(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional
    );

    public record GeoPointResponse(
        decimal Latitude,
        decimal Longitude
    );

    public record GuideInfoResponse(
        Guid GuideUserId,
        string FirstName,
        string LastName,
        string? ProfileImageUrl,
        decimal AverageRating,
        int ReviewCount,
        int CompletedToursCount
    );
}
