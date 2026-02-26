using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class DeviceRepository(ApplicationDbContext context) : IDeviceRepository
{
    public async Task AddDeviceAsync(UserDevice device, CancellationToken cancellationToken)
    {
        await context.UserDevices.AddAsync(device, cancellationToken);
    }
}
