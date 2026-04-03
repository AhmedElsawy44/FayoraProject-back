using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide);
    public Task<TourGuide?> GetGuideByIdAsync(Guid id, bool IsReadOnly, CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
