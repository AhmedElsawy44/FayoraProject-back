using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entitties.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class DeviceRepository(ApplicationDbContext context) : IDeviceRepository
{
    public void AddDevice(UserDevice device) => context.UserDevices.Add(device);

    public Task<UserDevice?> GetDeviceByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default, bool IsTracking = false)
    {
        if (IsTracking)
        {
            return context.UserDevices.FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
        }
        else
        {
            return context.UserDevices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
        }
    }

    public Task<UserDevice?> GetDeviceByIdAsync(string deviceId, CancellationToken cancellationToken = default, bool isTracking = false)
    {
        if (isTracking)
        {
            return context.UserDevices.FindAsync(deviceId).AsTask();
        }
        else
        {
            return context.UserDevices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
        }
    }

    public Task<UserDevice?> GetDeviceByUserIdAndDeviceIdAsync(Guid userId, string deviceId, CancellationToken cancellationToken = default, bool isTracking = false)
    {
        if (isTracking)
        {
            return context.UserDevices.FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId, cancellationToken);
        }
        else
        {
            return context.UserDevices.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId, cancellationToken);
        }
    }

    public Task<bool> IsBannedAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return context.UserDevices
        .AnyAsync(d => d.DeviceId == deviceId && d.IsBanned, cancellationToken);
    }

    public Task<bool> IsDeviceExistAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return context.UserDevices.AnyAsync(d => d.DeviceId == deviceId, cancellationToken);
    }
}
