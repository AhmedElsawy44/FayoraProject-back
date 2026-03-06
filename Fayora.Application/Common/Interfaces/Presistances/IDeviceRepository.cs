using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IDeviceRepository
{
    void AddDevice(UserDevice device);
    Task<UserDevice?> GetDeviceByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default, bool IsTracking = false);
    Task<UserDevice?> GetDeviceByIdAsync(string deviceId, CancellationToken cancellationToken = default, bool isTracking = false);
    Task<bool> IsDeviceExistAsync(string deviceId, CancellationToken cancellationToken = default);

    Task<UserDevice?> GetDeviceByUserIdAndDeviceIdAsync(Guid userId, string deviceId, CancellationToken cancellationToken = default, bool isTracking = false);
    Task<bool> IsBannedAsync(string deviceId, CancellationToken cancellationToken = default);
}
