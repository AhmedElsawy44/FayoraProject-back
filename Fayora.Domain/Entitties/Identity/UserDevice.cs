namespace Fayora.Domain.Entities.Identity;

public class UserDevice : BaseEntity<int>
{
    public Guid UserId { get; init; } = Guid.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string FCMToken { get; private set; } = string.Empty;
    public string DeviceLanguage { get; private set; } = string.Empty;
    public bool IsBanned { get; private set; } = false;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastUsedAt { get; private set; } = DateTimeOffset.UtcNow;

    public UserDevice(Guid userId, string deviceId, string fcmToken, string deviceLanguage)
    {
        UserId = userId;
        DeviceId = deviceId;
        FCMToken = fcmToken;
        DeviceLanguage = deviceLanguage;
    }

    public void UpdateInfo(string? newFcmToken = null, string? newLanguage = null)
    {
        FCMToken = newFcmToken ?? FCMToken;
        DeviceLanguage = newLanguage ?? DeviceLanguage;

        LastUsedAt = DateTimeOffset.UtcNow;
    }

    public void Ban() => IsBanned = true;

    private UserDevice() { }
}