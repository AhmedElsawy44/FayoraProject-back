using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithApple;

public record LoginWithAppleCommand(
    string IdToken,
    string FirstName,
    string LastName,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage) : ICommand<Result<LoginWithAppleResult>>, ICheckBannedRequest;