using Fayora.Domain.Entities.Identity;

public class PasswordResetToken : BaseEntity<int>
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }

    public bool IsValid => !IsUsed && !IsRevoked && DateTimeOffset.UtcNow <= ExpiresAt;

    public User User { get; private set; } = null!;

    private PasswordResetToken() { }

    internal static PasswordResetToken Create(Guid userId, string tokenHash, TimeSpan expirationMinutes)
    {
        return new PasswordResetToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(expirationMinutes),
            IsUsed = false,
            IsRevoked = false
        };
    }

    public void Consume()
    {
        IsUsed = true;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}
