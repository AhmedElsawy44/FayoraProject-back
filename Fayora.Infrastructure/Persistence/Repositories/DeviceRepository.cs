using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class DeviceRepository(ApplicationDbContext context) : BaseRepository<UserDevice, int>(context), IDeviceRepository
{
    public void AddDevice(UserDevice device) => Add(device);

    public Task<UserDevice?> GetDeviceByDeviceIdAsync(string deviceId, CancellationToken cancellationToken, bool IsTracking = false) => GetSingleAsync(x => x.DeviceId == deviceId, cancellationToken, IsTracking);

    public Task<UserDevice?> GetDeviceByIdAsync(string deviceId, CancellationToken cancellationToken, bool isTracking = false) => GetSingleAsync(d => d.DeviceId == deviceId, cancellationToken, isTracking);

    public Task<bool> IsDeviceExistAsync(string deviceId, CancellationToken cancellationToken) => IsExistAsync(d => d.DeviceId == deviceId, cancellationToken);
}
