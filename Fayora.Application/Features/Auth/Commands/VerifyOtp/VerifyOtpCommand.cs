using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand(Guid UserId, string? Email, string? PhoneNumber, string Code, string DeviceId, string FcmToken) : IRequest<Result<AuthResult>>;
