using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordCode;

public record VerifyResetPasswordCodeCommand(
    string? Email,
    string? PhoneNumber,
    string DeviceId,
    string Code) : IRequest<Result<string>>, ICheckBannedRequest
{
    public string Identity => (Email ?? PhoneNumber ?? "").Trim();
    public bool IsEmail => !string.IsNullOrWhiteSpace(Email);
}
