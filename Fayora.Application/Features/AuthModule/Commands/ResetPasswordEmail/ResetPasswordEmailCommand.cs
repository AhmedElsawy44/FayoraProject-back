using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResetPasswordEmail;

public record ResetPasswordEmailCommand(
    string Email,
    string ResetToken,
    string NewPassword,
    string DeviceId) : IRequest<Result<Unit>>, ICheckBannedRequest;