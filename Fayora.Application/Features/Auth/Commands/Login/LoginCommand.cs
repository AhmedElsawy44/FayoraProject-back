using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string? Email,
    string? PhoneNumber,
    string Password,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
    ) : IRequest<Result<AuthResult>>, ICheckBannedRequest

{
    public string Identity => (Email ?? PhoneNumber ?? "").Trim();
    public bool IsEmail => !string.IsNullOrWhiteSpace(Email);
}


