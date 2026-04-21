using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyEmail;

public record VerifyEmailCommand(
    string Email,
    string Code,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    Language DeviceLanguage) : ICommand<Result<VerifyEmailResult>>, ICheckBannedRequest;
