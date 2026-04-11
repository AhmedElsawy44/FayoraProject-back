using Fayora.Domain.Entities.Shared;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById;

public record GetTourGuideByIdResult
(
    Guid Id,
    Guid UserId,
    decimal BaseRate,
    int YearsOfExperience,
    string LicenseNumber,
    DateOnly LicenseExpiryDate,
    string TaxRegistrationNumber,
    DateOnly? TaxRegistrationDate,
    string CurrencyCode,
    int ReviewCount,
    float AverageRating,
    GuideStatus Status,
    bool IsAvailableForBooking,
    bool IsOnline,
    int CompletedToursCount,
    bool IsSuperGuide,
    decimal CancellationRate,
    TransportInfo? TransportInfo,
    float ResponseRate,
    List<City> GuideCities,
    List<GuideTourPackageSummaryDto> GuideTourPackages
);

public record GuideTourPackageSummaryDto(
    Guid Id,
    string Title,
    string? MainImageUrl,
    decimal AdultPrice,
    int DurationHours,
    TourType TourType,
    int AvailableSpots
);