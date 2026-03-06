using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string? Email,
    string? PhoneNumber,
    string Password,
    string DeviceId
    ) : IRequest<Result<RegisterResult>>, ICheckBannedRequest
{
    public string Identity => (Email ?? PhoneNumber ?? "").Trim();
    public bool IsEmail => !string.IsNullOrWhiteSpace(Email);
}