using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyRegisterCode;

public record VerifyRegisterCodeCommand(Guid UserId, string? Email, string? PhoneNumber, string Code, string DeviceId, string FcmToken, string? SimCountryIsoCode, string TimeZone, string DeviceLanguage) : IRequest<Result<AuthResult>>, ICheckBannedRequest
{
    public string Identity => (Email ?? PhoneNumber ?? "").Trim();
    public bool IsEmail => !string.IsNullOrWhiteSpace(Email);
}
