using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide);
    public Task<TourGuide?> GetGuideByIdAsync(Guid id, GuideQueryOptions options, CancellationToken cancellationToken);

    public Task<bool> TourGuideExistAsync(Guid id, CancellationToken cancellationToken);

    public record GuideQueryOptions(
        bool ReadOnly = true
    );
}
