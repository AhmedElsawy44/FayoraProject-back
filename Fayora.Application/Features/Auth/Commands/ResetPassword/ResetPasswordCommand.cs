using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string? Email,
    string? PhoneNumber,
    string ResetToken,
    string NewPassword) : IRequest<Result<Unit>>;