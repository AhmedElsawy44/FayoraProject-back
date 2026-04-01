namespace Fayora.Contracts.AuthModule.UpdateAccount;

public record UpdateAccountRequest(
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    string? Gender,
    string? NationalityCode,
    string? ProfileImageUrl,
    string? Description,
    string? PreferredLanguage,
    List<UserLanguageDto> UserLanguages,
    string? TimeZone
);

public record UserLanguageDto(
    decimal ProficiencyLevel,
    string Language
);