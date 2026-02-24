using ErrorOr;
using Fayora.Domain.Common;
using Fayora.Domain.Enums;
using System;

namespace Fayora.Domain.Entities.Security; // أو Identity حسب ما تحب

public class BannedItem : BaseEntity<int>
{
    public Guid? UserId { get; private set; }

    public BanType BanType { get; private set; }

    public string BanValue { get; init; }

    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsCurrentlyBanned =>
        IsActive && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTimeOffset.UtcNow);

    private BannedItem() { }

    public static ErrorOr<BannedItem> Create(
        BanType banType,
        string banValue,
        string reason,
        Guid? userId = null,
        DateTimeOffset? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(banValue))
            return Error.Validation("BannedItem.InvalidValue", "Ban value cannot be empty.");

        return new BannedItem
        {
            BanType = banType,
            BanValue = banValue,
            Reason = string.IsNullOrWhiteSpace(reason) ? "No reason provided" : reason,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
            IsActive = true
        };
    }

    public ErrorOr<Success> RevokeBan(string revokeReason)
    {
        if (!IsActive)
            return Error.Conflict("BannedItem.AlreadyRevoked", "This ban is already inactive or revoked.");

        IsActive = false;

        Reason += $" | Revoked Reason: {revokeReason}";

        return Result.Success;
    }

    public ErrorOr<Success> ExtendBan(DateTimeOffset newExpirationDate)
    {
        if (newExpirationDate <= DateTimeOffset.UtcNow)
            return Error.Validation("BannedItem.InvalidDate", "Expiration date must be in the future.");

        ExpiresAt = newExpirationDate;
        IsActive = true;

        return Result.Success;
    }
}