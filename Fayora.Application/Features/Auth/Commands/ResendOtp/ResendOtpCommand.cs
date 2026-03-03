using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResendRegisterOtp;

public record ResendOtpCommand(Guid UserId, string? Email, string? PhoneNumber, string? SimCountryIsoCode, OtpPurpose OtpPurpose) : IRequest<Result<Unit>>;
