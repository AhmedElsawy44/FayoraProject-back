using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyResetPasswordOtp;

public record VerifyResetPasswordOtpCommand(
    string? Email,
    string? PhoneNumber,
    string Code) : IRequest<Result<string>>;
