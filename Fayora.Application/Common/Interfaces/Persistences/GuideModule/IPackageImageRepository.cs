using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageImageRepository
{
    void AddPackageImages(IEnumerable<PackageImage> guideImages);
    void RemovePackageImages(IEnumerable<PackageImage> guideImages);
    Task<List<PackageImage>> GetPackageImages(Guid packageId, CancellationToken cancellationToken);
}
