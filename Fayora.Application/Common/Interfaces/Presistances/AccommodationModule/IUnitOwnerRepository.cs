using Fayora.Domain.Entitties.Accommodation;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IUnitOwnerRepository
{
    public void AddOwner(UnitOwner owner);
    public Task<bool> IsOwnerExistAsync(Guid userId);
    public Task<UnitOwner?> GetOwnerByUserIdAsync(Guid userId, bool isReadOnly = true, CancellationToken cancellationToken = default); 
}
