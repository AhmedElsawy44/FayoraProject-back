using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(
    string Email,
    string Code,
    string DeviceId,
    string FcmToken,
    string? SimCountryIsoCode,
    string TimeZone,
    string DeviceLanguage) : IRequest<Result<VerifyEmailResult>>, ICheckBannedRequest;