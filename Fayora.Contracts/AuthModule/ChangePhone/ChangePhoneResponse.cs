namespace Fayora.Contracts.AuthModule.ChangePhone;

public record ChangePhoneResponse(
    Guid UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
