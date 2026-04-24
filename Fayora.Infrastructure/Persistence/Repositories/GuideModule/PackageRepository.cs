using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class PackageRepository(ApplicationDbContext context) : IPackageRepository
{
    public void AddPackage(GuidePackage package)
    {
        context.GuideTourPackages.Add(package);
    }

    public async Task<GuidePackage?> GetPackageByIdAsync(
        Guid packageId,
        PackageQueryOptions options,
        CancellationToken cancellationToken)
    {
        IQueryable<GuidePackage> query = context.GuideTourPackages;
        if (options.ReadOnly)
        {
            query = query.AsNoTracking();
        }

        return await query
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);
    }
}