using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyAdminLogin;

public record VerifyAdminLoginCommand(
    string Email,
    string Code,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    Language DeviceLanguage) : ICommand<Result<VerifyAdminLoginResult>>, ICheckBannedRequest;
