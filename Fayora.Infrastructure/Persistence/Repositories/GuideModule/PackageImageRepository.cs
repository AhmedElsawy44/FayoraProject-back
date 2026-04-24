using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class PackageImageRepository(ApplicationDbContext context) : IPackageImageRepository
{
    public void AddPackageImages(IEnumerable<PackageImage> guideImages)
    {
        context.PackageImages.AddRange(guideImages);
    }
}

