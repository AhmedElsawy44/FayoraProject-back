namespace Fayora.Application.Common.Interfaces.Services;

public interface IUserDeviceManager
{
    Task UpsertDeviceAsync(Guid userId, string deviceId, string fcmToken, string deviceLanguage, CancellationToken cancellationToken = default);
}