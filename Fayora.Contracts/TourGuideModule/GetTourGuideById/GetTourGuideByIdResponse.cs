namespace Fayora.Contracts.TourGuideModule.GetTourGuideById;

public record GetTourGuideByIdResponse(
    Guid Id,
    Guid UserId,
    decimal BaseRate,
    decimal DiscountedBaseRate,
    int YearsOfExperience,
    string LicenseNumber,
    DateTime LicenseExpiryDate,
    string? TaxRegistrationNumber,
    DateTime? TaxRegistrationDate,
    string CurrencyCode,
    int ReviewCount,
    decimal AverageRating,
    string Status,
    bool IsAvailableForBooking,
    bool IsOnline,
    int CompletedToursCount,
    bool IsSuperGuide,
    decimal CancellationRate,
    string? TransportInfo,
    float ResponseRate,
    IReadOnlyCollection<CityResponse> Cities,
    IReadOnlyCollection<GuideTourPackageSummaryResponse> TourPackages
);

public record CityResponse(
    int CityId,
    string Name,
    string CountryCode,
    decimal Latitude,
    decimal Longitude
);

public record GuideTourPackageSummaryResponse(
    Guid Id,
    string Title,
    string? MainImageUrl,
    decimal AdultPrice,
    int DurationHours,
    string TourType,
    int AvailableSpots
);
