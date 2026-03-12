using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IUnitOwnerRepository
{
    public void AddOwner(UnitOwner owner);
    public Task<UnitOwner?> GetOwnerByUserIdAsync(Guid userId, bool isReadOnly = true, CancellationToken cancellationToken = default);
}
