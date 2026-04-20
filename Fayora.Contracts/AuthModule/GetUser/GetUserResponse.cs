namespace Fayora.Contracts.AuthModule.GetUser;

public record GetUserResponse
(
    Guid Id,
    string? ProfileImageUrl,
    string? Email,
    string? PhoneNumber,
    bool IsEmailVerified,
    bool IsPhoneNumberVerified,
    string FirstName,
    string LastName,
    string? NationalityCode,
    string? Gender,
    DateOnly? BirthDate,
    string? Description,
    string? PreferredLanguage,
    List<LanguageProficiencyDto> LanguageProficiencies
);

public record LanguageProficiencyDto(string Language, decimal Proficiency);

