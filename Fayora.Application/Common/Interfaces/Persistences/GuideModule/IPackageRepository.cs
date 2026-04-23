using Fayora.Domain.Entities.TourGuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface IPackageRepository
{
    void AddPackage(GuidePackage package, CancellationToken cancellationToken);
    Task<GuidePackage> GetPackageByIdAsync(Guid packageId, PackageQueryOptions options, CancellationToken cancellationToken);
    void DeletePackage(GuidePackage package);

    public record PackageQueryOptions(
        bool ReadOnly = true
    );
}
