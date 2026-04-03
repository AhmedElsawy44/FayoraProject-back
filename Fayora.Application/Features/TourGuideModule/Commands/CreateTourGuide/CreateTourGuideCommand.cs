using Fayora.Contracts.AuthModule.UpdateAccount;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public record CreateTourGuideCommand
(
    string DeviceId,
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
) : IRequest<Result<CreateTourGuideResult>>;
