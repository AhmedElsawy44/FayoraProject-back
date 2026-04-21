using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public record CreateTourGuideCommand
(
    DateOnly? BirthDate,
    Gender? Gender,
    string DeviceId,
    string ProfilePictureUrl,
    string Description,
    PricingUnit PricingUnit,
    decimal BaseRate,
    int YearsOfExperience,
    string NationalityCode,
    List<Guid> CityIds,
    Language? PreferredLanguage,
    List<UserLanguageProficiencyDto> TourGuideLanguages,
    string? TimeZone
) : ICommand<Result<CreateTourGuideResult>>;

public record UserLanguageProficiencyDto(
    Language Language,
    decimal ProficiencyLevel);
