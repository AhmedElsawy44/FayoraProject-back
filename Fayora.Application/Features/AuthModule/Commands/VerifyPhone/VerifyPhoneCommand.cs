using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyPhone;

public record VerifyPhoneCommand(
string PhoneNumber,
string Code,
string DeviceId,
string FcmToken,
string? SimCountryIsoCode,
string TimeZone,
string DeviceLanguage) : IRequest<Result<VerifyPhoneResult>>, ICheckBannedRequest;
