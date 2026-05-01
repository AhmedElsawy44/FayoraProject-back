using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithSocial;

public record LoginWithSocialCommand(
    string Token,
    string? FirstName,
    string? LastName,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    Language DeviceLanguage,
    IdentityProvider IdentityProvider
    ) : ICommand<Result<LoginWithSocialResult>>, ICheckBannedRequest;