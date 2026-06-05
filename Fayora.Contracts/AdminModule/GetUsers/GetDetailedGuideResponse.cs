namespace Fayora.Contracts.AdminModule.GetUsers;

public record GetDetailedGuideResponse(
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    decimal? BaseRate,
    string? PricingUnit,
    int? YearsOfExperience,
    string? LicenseNumber,
    DateOnly? LicenseExpiryDate,
    bool IsSuperGuide,
    string? ProfessionalLicenseUrl,
    string CancellationPolicy,
    string CurrencyCode,
    decimal AverageRating,
    int ReviewCount,
    int CompletedToursCount,
    bool IsAvailableForBooking,
    int Views,
    decimal ResponseRate,
    decimal CancellationRate,
    string Status,
    DateTimeOffset CreatedAt,
    string? AdminNotes,
    List<string> CoveredCities,
    List<CompanyPackageDto> AssociatedPackages
);
