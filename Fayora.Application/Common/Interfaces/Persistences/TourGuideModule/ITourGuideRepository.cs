using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide);
    public Task<TourGuide?> GetGuideByIdAsync(Guid id, GuideQueryOptions options, CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    public record GuideQueryOptions(
        bool ReadOnly = true,
        bool IncludeCities = false,
        bool IncludeTourPackages = false
    );
}
