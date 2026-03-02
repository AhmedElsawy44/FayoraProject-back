using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string? Email, string? PhoneNumber, string Password, string? SimCountryIsoCode, string TimeZone, string DeviceId, string DeviceLanguage) : IRequest<Result<RegisterResult>>;

