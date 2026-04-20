using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithGoogle;

public record LoginWithGoogleCommand
(
    string IdToken,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage
) : ICommand<Result<LoginWithGoogleResult>>, ICheckBannedRequest;