using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuidePackageRepository
{
    void AddPackageAsync(GuideTourPackage package, CancellationToken cancellationToken);
    Task<GuideTourPackage> GetPackageByIdAsync(Guid packageId, bool IsReadOnly, CancellationToken cancellationToken);
    void DeletePackage(GuideTourPackage package);
}
