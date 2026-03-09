namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface IUserDeviceManager
{
    Task UpsertDeviceAsync(Guid userId, string deviceId, string fcmToken, string deviceLanguage, CancellationToken cancellationToken = default);
}