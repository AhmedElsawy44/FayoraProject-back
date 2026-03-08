namespace Fayora.Contracts.Auth.Responses;

public record PhoneLoginResponse(
    string UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
