using Fayora.Contracts.AuthModule.UpdateAccount;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public record UpdateAccountCommand(
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    string? Gender,
    string? NationalityCode,
    string? ProfileImageUrl,
    string? Description,
    string? PreferredLanguage,
    List<UserLanguageDto> UserLanguages,
    string? TimeZone) : ICommand<Result<Success>>;
