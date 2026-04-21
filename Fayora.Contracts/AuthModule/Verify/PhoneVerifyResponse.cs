namespace Fayora.Contracts.AuthModule.Verify;

public record PhoneVerifyResponse
(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? ImageUrl,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

