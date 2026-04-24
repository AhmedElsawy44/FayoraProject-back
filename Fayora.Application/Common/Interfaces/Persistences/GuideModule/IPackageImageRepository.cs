using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IPackageImageRepository
{
    void AddPackageImages(IEnumerable<PackageImage> guideImages);
}
