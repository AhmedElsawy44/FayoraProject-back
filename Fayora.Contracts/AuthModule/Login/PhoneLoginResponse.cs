namespace Fayora.Contracts.AuthModule.Login;

public record PhoneLoginResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ProfileImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
