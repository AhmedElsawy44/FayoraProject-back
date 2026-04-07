namespace Fayora.Contracts.AuthModule.ConfirmChangePhone;

public record ConfirmChangePhoneResponse
(
    Guid Id,
    string PhoneNumber,
    string AccessToken,
    int ExpiresIn
);
