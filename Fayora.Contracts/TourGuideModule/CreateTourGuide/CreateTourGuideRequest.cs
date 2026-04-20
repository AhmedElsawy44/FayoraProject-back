using Fayora.Contracts.AuthModule.UpdateAccount;

namespace Fayora.Contracts.TourGuideModule.CreateTourGuide;

public record CreateTourGuideRequest
(
    string ProfilePictureUrl,
    string Description,
    string PricingUnit,
    decimal BaseRate,
    int YearsOfExperience,
    string LicenseNumber,
    DateOnly LicenseExpiryDate,
    string CurrencyCode,
    List<Guid> CityIds,
    string PreferredLanguage,
    List<UserLanguageDto> TourGuideLanguages
);
