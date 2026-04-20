using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithFacebook;

public record LoginWithFacebookCommand(
    string AccessToken,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
) : ICommand<Result<LoginWithFacebookResult>>, ICheckBannedRequest;