namespace Fayora.Contracts.AuthModule.UpdateAccount;

public record UpdateAccountRequest(
    string? FirstName,
    string? LastName,
    DateOnly? BirthDate,
    string? Gender,
    string? NationalityCode,
    string? ProfileImageUrl,
    string? Description,
    string? PreferredLanguage,
    string? TimeZone
);