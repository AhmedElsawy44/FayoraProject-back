using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public record GetUserResult(
    Guid Id,
    string? ProfileImageUrl,
    string? Email,
    string? PhoneNumber,
    bool IsEmailVerified,
    bool IsPhoneNumberVerified,
    string FirstName,
    string LastName,
    string? NationalityCode,
    Gender? Gender,
    DateOnly? BirthDate,
    string? Description,
    Language? PreferredLanguage,
    List<UserLanguageProficiency> LanguageProficiencies
);
