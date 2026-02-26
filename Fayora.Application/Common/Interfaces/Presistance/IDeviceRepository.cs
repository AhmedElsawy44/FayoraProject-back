using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IDeviceRepository
{
    Task AddDeviceAsync(UserDevice device, CancellationToken cancellationToken);
}
