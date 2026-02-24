namespace Fayora.Domain.Entitties.Identity;

public class UserArchive : BaseEntity<int>
{
    public Guid OriginalUserId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string DeletionReason { get; init; } = string.Empty;
    public string DeletedBy { get; init; } = string.Empty;
    public bool IsRejoinable { get; init; }
    public string FullUserDataBackup { get; init; } = string.Empty;
    public DateTimeOffset OriginalCreatedAt { get; init; }
    public DateTimeOffset ArchivedAt { get; init; }

    private UserArchive() { }

    public  UserArchive(
        Guid originalUserId,
        string? email,
        string? phone,
        string fullName,
        string deletionReason,
        string deletedBy,
        bool isRejoinable,
        string fullUserDataBackup,
        DateTimeOffset originalCreatedAt)
    {
        OriginalUserId = originalUserId;
        Email = email;
        Phone = phone;
        FullName = fullName;
        DeletionReason = deletionReason;
        DeletedBy = deletedBy;
        IsRejoinable = isRejoinable;
        FullUserDataBackup = fullUserDataBackup;
        OriginalCreatedAt = originalCreatedAt;
        ArchivedAt = DateTimeOffset.UtcNow;
    }
}