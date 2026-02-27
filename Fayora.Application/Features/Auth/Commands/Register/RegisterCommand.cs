using Fayora.Application.Common.Authentication;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string? Email, string? PhoneNumber, string Password, string? SimCountryIsoCode, string TimeZone, string DeviceId, string FcmToken, string DeviceType, string DeviceModel, string DeviceLanguage) : IRequest<Result<AuthResult>>;

