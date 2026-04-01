using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Domain.Entities.IdentityModule;

public class UserDevice : BaseEntity<int>
{
    public Guid UserId { get; init; } = Guid.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string FCMToken { get; private set; } = string.Empty;
    public Language DeviceLanguage { get; private set; } = Language.English;
    public bool IsBanned { get; private set; } = false;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastUsedAt { get; private set; } = DateTimeOffset.UtcNow;

    public UserDevice(Guid userId, string deviceId, string fcmToken, Language deviceLanguage)
    {
        UserId = userId;
        DeviceId = deviceId;
        FCMToken = fcmToken;
        DeviceLanguage = deviceLanguage;
    }

    public void UpdateInfo(string newFcmToken, Language newLanguage)
    {
        FCMToken = newFcmToken ?? FCMToken;
        DeviceLanguage = newLanguage;

        LastUsedAt = DateTimeOffset.UtcNow;
    }

    public void Ban() => IsBanned = true;

    private UserDevice() { }
}