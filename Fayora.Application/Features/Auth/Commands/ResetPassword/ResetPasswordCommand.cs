using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string? Email,
    string? PhoneNumber,
    string DeviceId,
    string ResetToken,
    string NewPassword) : IRequest<Result<Unit>>, ICheckBannedRequest
{
    public string Identity => (Email ?? PhoneNumber ?? "").Trim();
    public bool IsEmail => !string.IsNullOrWhiteSpace(Email);
}