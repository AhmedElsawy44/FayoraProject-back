namespace Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails
{
    public record PackageDetailsResult(
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
        List<PackageActivityDetailsResult> Activities,
        GeoPointDetailsResult MeetingPoint,
        GuideInfoDetailsResult GuideInfo,
        string CancellationPolicy,
        string TransportType,
        string? GuestRequirements,
        string? ArrivalNote,
        List<int> LocationIds,
        List<PackageOccurrenceResult> Occurrences
    );

    public record PackageActivityDetailsResult(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional
    );

    public record GeoPointDetailsResult(
        decimal Latitude,
        decimal Longitude
    );

    public record GuideInfoDetailsResult(
        Guid GuideUserId,
        string FirstName,
        string LastName,
        string? ProfileImageUrl,
        decimal AverageRating,
        int ReviewCount,
        int CompletedToursCount
    );

    public record PackageOccurrenceResult(
        Guid Id,
        DateOnly Date,
        int AvailableSeats
    );
}