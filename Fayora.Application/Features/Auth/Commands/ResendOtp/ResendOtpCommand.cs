using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResendOtp;

public record ResendOtpCommand(string? Email, string? PhoneNumber, OtpPurpose OtpPurpose) : IRequest<Result<Unit>>;
