using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuidePackageRepository
{
    Task AddPackageAsync(GuideTourPackage package, CancellationToken cancellationToken);
}
