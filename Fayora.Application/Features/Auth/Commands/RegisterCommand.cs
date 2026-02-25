using Fayora.Application.Common.Authentication;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands;

public record RegisterCommand(string FirstName, string LastName, string? Email, string? PhoneNumber, string Password, string? SimCountryIsoCode, string PreferredLanguage, string TimeZone) : IRequest<Result<AuthResult>>;