using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithApple;

public record LoginWithAppleCommand(
    string AccessToken,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage) : IRequest<Result<LoginWithAppleResult>>, ICheckBannedRequest;