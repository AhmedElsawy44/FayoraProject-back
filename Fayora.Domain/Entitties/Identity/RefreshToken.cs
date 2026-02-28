namespace Fayora.Domain.Entities.Identity;

public class RefreshToken : BaseEntity<int>
{
    public static readonly int ExpiryDays = 7;
    public Guid UserId { get; set; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; private set; }
    public string DeviceId { get; init; } = string.Empty;
    public string? IpAddress { get; init; }

    public RefreshToken(Guid userId, string token, string deviceId, string? ipAddress)
    {
        UserId = userId;
        Token = token;
        DeviceId = deviceId;
        IpAddress = ipAddress;
        ExpiresAt = DateTime.UtcNow.AddDays(ExpiryDays);

    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    private RefreshToken() { }
}
