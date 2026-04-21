namespace Fayora.Contracts.AuthModule.ConfirmChangePhone;

public record ConfirmChangePhoneResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
