using Fayora.Domain.Enums;

namespace Fayora.Domain.Entities.Identity;

public class BannedItem : BaseEntity<int>
{
    public Guid? UserId { get; private set; }
    public BanType BanType { get; init; } = BanType.DeviceId;
    public string BanValue { get; init; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    public bool IsCurrentlyBanned =>
        IsActive && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTimeOffset.UtcNow);

    private BannedItem() { }

    public BannedItem(Guid? userId, BanType banType, string banValue, string reason, DateTimeOffset? expiresAt = null)
    {
        UserId = userId;
        BanType = banType;
        BanValue = banValue;
        Reason = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : reason;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    public void RevokeBan(string revokeReason)
    {
        IsActive = false;
        Reason += $" | Revoked Reason: {revokeReason}";
    }

    public void ExtendBan(DateTimeOffset newExpirationDate)
    {
        ExpiresAt = newExpirationDate;
        IsActive = true;
    }
}