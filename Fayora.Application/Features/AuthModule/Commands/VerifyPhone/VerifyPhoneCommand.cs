using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public record VerifyPhoneCommand(
string PhoneNumber,
string Code,
string DeviceId,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
Language DeviceLanguage) : ICommand<Result<VerifyPhoneResult>>, ICheckBannedRequest;
