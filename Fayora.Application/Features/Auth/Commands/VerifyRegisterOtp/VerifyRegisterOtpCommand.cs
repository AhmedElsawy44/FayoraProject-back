using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyRegisterOtp;

public record VerifyRegisterOtpCommand(Guid UserId, string? Email, string? PhoneNumber, string? SimCountryIsoCode, string Code, string DeviceId, string FcmToken) : IRequest<Result<AuthResult>>;
