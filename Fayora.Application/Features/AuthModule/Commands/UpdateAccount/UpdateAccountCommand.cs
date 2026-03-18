using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public record UpdateAccountCommand(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    Gender? Gender,
    string? NationalityCode,
    string? ProfileImageUrl,
    string? Description,
    string? PreferredLanguage,
    string? TimeZone) : IRequest<Result<Success>>;
