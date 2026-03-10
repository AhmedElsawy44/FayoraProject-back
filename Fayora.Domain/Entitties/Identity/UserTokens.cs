using Fayora.Domain.Shared.IdentityModule;

namespace Fayora.Domain.Entitties.Identity;

public class UserTokens : BaseEntity<int>
{
    public static readonly int RefreshTokenExpiryDays = 7;
    public static readonly TimeSpan PasswordResetTokenExpiration = TimeSpan.FromMinutes(15);
    public Guid UserId { get; init; }
    public string HashedToken { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string DeviceId { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public TokenType TokenType { get; init; } = TokenType.RefreshToken;

    public bool IsValid => !RevokedAt.HasValue && DateTimeOffset.UtcNow <= ExpiresAt;

    public static UserTokens RefreshToken(Guid userId, string hashedToken, string deviceId, string? ipAddress)
    {
        return new UserTokens
        {
            UserId = userId,
            HashedToken = hashedToken,
            DeviceId = deviceId,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
            TokenType = TokenType.RefreshToken
        };
    }

    public static UserTokens PasswordResetToken(Guid userId, string tokenHash)
    {
        return new UserTokens
        {
            UserId = userId,
            HashedToken = tokenHash,
            ExpiresAt = DateTime.UtcNow.Add(PasswordResetTokenExpiration),
            TokenType = TokenType.PasswordResetToken
        };

    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    private UserTokens() { }
}
