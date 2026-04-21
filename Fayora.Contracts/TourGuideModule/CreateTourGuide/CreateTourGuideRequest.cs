using Fayora.Contracts.AuthModule.UpdateAccount;

namespace Fayora.Contracts.TourGuideModule.CreateTourGuide;

public record CreateTourGuideRequest
(
    DateOnly BirthDate,
    string Gender,
    string ProfilePictureUrl,
    string Description,
    string PricingUnit,
    decimal BaseRate,
    int YearsOfExperience,
    string LicenseNumber,
    DateOnly LicenseExpiryDate,
    string NationalityCode,
    List<Guid> CityIds,
    string PreferredLanguage,
    string TimeZone,
    List<UserLanguageDto> TourGuideLanguages
);
