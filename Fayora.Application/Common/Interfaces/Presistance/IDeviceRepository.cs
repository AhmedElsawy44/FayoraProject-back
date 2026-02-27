using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IDeviceRepository
{
    void AddDevice(UserDevice device);
    Task<UserDevice?> GetDeviceByDeviceIdAsync(string deviceId, CancellationToken cancellationToken, bool IsTracking = false);
    Task<bool> IsDeviceExistAsync(string deviceId, CancellationToken cancellationToken);
}
