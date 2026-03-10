using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Domain.Entitties.Identity;

namespace Fayora.Infrastructure.Services.AuthServices;

public class UserDeviceManager(IDeviceRepository deviceRepository) : IUserDeviceManager
{
    public async Task UpsertDeviceAsync(Guid userId, string deviceId, string fcmToken, string deviceLanguage, CancellationToken cancellationToken = default)
    {
        var device = await deviceRepository.GetDeviceByUserIdAndDeviceIdAsync(userId, deviceId, cancellationToken, isTracking: true);

        if (device is null)
        {
            device = new UserDevice(userId, deviceId, fcmToken, deviceLanguage);
            deviceRepository.AddDevice(device);
        }
        else
        {
            device.UpdateInfo(fcmToken, deviceLanguage);
        }
    }
}