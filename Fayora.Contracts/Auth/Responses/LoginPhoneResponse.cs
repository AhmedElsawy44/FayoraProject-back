namespace Fayora.Contracts.Auth.Responses;

public record LoginPhoneResponse(
    string UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
