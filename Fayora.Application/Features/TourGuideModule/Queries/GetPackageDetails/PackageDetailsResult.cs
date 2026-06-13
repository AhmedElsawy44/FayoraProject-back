using Fayora.Contracts.TourGuideModule.GetPackageDetails;

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
        int NumOfDays,
        int NumOfNights,
        string MainImageUrl,
        List<string> ImageUrls,
        List<int> IncludedItemIds,
        List<int>? ExcludedItemIds,
        List<PackageActivityDetailsResult> Activities,
        List<PackageNightDetailsResult> Nights,
        GeoPointDetailsResult MeetingPoint,
        GuideInfoDetailsResult GuideInfo,
        string CancellationPolicy,
        string TransportType,
        string? GuestRequirements,
        string? ArrivalNote,
        List<int> LocationIds,
        List<PackageOccurrenceResult> Occurrences,
        List<OptionalActivityResponse>? OptionalActivities
    );

    public record PackageActivityDetailsResult(
        string Description,
        TimeOnly ActivityTime,
        bool IsOptional,
        int? LocationId
    );

    public record PackageNightDetailsResult(
    Guid Id,
    int NightNumber,
    DateOnly NightDate,
    Guid? HousingUnitId,         // لو من الـ Accommodation System
    Guid? PackageAccommodationId  // لو خارج الـ System التور جيد هو اللي عامل مكان الاقامه
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