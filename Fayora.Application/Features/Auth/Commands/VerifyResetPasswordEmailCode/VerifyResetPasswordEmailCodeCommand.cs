using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordEmailCode;

public record VerifyResetPasswordEmailCodeCommand(
string Email,
string DeviceId,
string Code) : IRequest<Result<string>>, ICheckBannedRequest;
