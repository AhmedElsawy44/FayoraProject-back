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
        int NumOfDays,
        int NumOfNights,
        string MainImageUrl,
        List<string> ImageUrls,
        List<int> IncludedItemIds,
        List<int>? ExcludedItemIds,
        List<PackageActivityResponse> Activities,
        List<PackageNightResponse> Nights,
        List<int> LocationIds,
        GeoPointResponse MeetingPoint,
        GuideInfoResponse GuideInfo
    );

    public record PackageActivityResponse(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId
    );

    public record PackageNightResponse(
    Guid Id,
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,
    Guid? PackageAccommodationId
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
