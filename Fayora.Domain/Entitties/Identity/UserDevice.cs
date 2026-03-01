namespace Fayora.Domain.Entities.Identity;

public class UserDevice : BaseEntity<int>
{
    public Guid UserId { get; init; } = Guid.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string FCMToken { get; private set; } = string.Empty;
    public string DeviceLanguage { get; private set; } = string.Empty;
    public DateTimeOffset LastUsedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public UserDevice(Guid userId, string deviceId, string fcmToken, string deviceLanguage)
    {
        UserId = userId;
        DeviceId = deviceId;
        FCMToken = fcmToken;
        DeviceLanguage = deviceLanguage;
    }

    public void UpdateUsage(string newLanguage)
    {
        DeviceLanguage = newLanguage;
        LastUsedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateFcmToken(string newFcmToken)
    {
        FCMToken = newFcmToken;
        LastUsedAt = DateTimeOffset.UtcNow;
    }

    private UserDevice() { }
}