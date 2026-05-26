namespace Fayora.Application.Features.AdminModule.Queries.GetTourGuideVerificationDetails;

public record GetTourGuideVerificationDetailsResponse(
    Guid UserId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? LicenseNumber,
    string? ProfessionalLicenseUrl,
    DateOnly? LicenseExpiryDate,
    int? YearsOfExperience,
    decimal? BaseRate,
    string? PricingUnit,
    List<string> TransportFeatures,
    List<string> CoveredCities
);
