using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public record UpdateAccountCommand(
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    Gender? Gender,
    string? NationalityCode,
    string? ProfileImageUrl,
    string? Description,
    Language? PreferredLanguage,
    List<UserLanguageProficiencyDto> UserLanguages,
    string? TimeZone) : ICommand<Result<Success>>;

public record UserLanguageProficiencyDto(
    Language Language,
    decimal ProficiencyLevel);
