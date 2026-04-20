using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public record VerifyPhoneCommand(
string PhoneNumber,
string Code,
string DeviceId,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage) : ICommand<Result<VerifyPhoneResult>>, ICheckBannedRequest;
