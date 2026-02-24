namespace Fayora.Domain.Entitties.Identity;

public class UserDevice : BaseEntity<int>
{
    public Guid UserId { get; init; }
    public string FCMToken { get; private set; }
    public string DeviceType { get; init; }
    public string DeviceModel { get; init; }
    public string DeviceLanguage { get; private set; }
    public DateTimeOffset LastUsedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    internal UserDevice(Guid userId, string fcmToken, string deviceType, string deviceModel, string deviceLanguage)
    {
        UserId = userId;
        FCMToken = fcmToken;
        DeviceType = deviceType;
        DeviceModel = deviceModel;
        DeviceLanguage = deviceLanguage;
    }

    internal void UpdateUsage(string newLanguage)
    {
        DeviceLanguage = newLanguage;
        LastUsedAt = DateTimeOffset.UtcNow;
    }

    private UserDevice() { }
}