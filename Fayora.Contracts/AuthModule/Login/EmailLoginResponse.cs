namespace Fayora.Contracts.AuthModule.Login;

public record EmailLoginResponse
(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? ProfileImageUrl,
    string? AccessToken,
    string? RefreshToken,
    int ExpiresIn,
    bool RequiresOtp = false
);
