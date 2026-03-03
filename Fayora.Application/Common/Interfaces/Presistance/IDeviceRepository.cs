using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IDeviceRepository
{
    void AddDevice(UserDevice device);
    Task<UserDevice?> GetDeviceByDeviceIdAsync(string deviceId, CancellationToken cancellationToken, bool IsTracking = false);
    Task<UserDevice?> GetDeviceByIdAsync(string deviceId, CancellationToken cancellationToken, bool isTracking = false);
    Task<bool> IsDeviceExistAsync(string deviceId, CancellationToken cancellationToken);
}
