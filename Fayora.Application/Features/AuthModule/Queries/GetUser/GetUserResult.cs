using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public record GetUserResult(
    Guid UserId,
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
