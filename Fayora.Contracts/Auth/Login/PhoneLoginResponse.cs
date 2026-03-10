namespace Fayora.Contracts.Auth.Login;

public record PhoneLoginResponse(
    string UserId,
    string PhoneNumber,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
