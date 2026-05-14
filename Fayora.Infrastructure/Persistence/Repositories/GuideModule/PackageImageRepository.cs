using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class PackageImageRepository(ApplicationDbContext context) : IPackageImageRepository
{
    public void AddPackageImages(IEnumerable<PackageImage> guideImages)
    {
        context.PackageImages.AddRange(guideImages);
    }

    public Task<List<PackageImage>> GetPackageImages(Guid packageId, CancellationToken cancellationToken)
    {
        return context.PackageImages.Where(x => x.PackageId == packageId).ToListAsync(cancellationToken);
    }
}

