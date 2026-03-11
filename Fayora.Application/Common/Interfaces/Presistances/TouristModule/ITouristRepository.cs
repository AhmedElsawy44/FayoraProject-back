using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Common.Interfaces.Presistances.TouristModule;

public interface ITouristRepository
{
    public void AddTourist(TouristProfile touristProfile);
    public Task<TouristProfile?> GetTouristByUserIdAsync(Guid userId, bool isReadOnly = true, CancellationToken cancellationToken = default!);
    public Task<bool> IsTouristProfileExistAsync(Guid userId, CancellationToken cancellationToken = default!);
}
