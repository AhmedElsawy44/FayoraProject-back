using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Entities.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;

public class TourGuidePackageRepository(ApplicationDbContext context) : IPackageRepository
{
    public void AddPackage(GuidePackage package, CancellationToken cancellationToken)
    {
        context.GuideTourPackages.Add(package);
    }

    public async Task<GuidePackage?> GetPackageByIdAsync(
        Guid packageId,
        IPackageRepository.PackageQueryOptions options,
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

    public void DeletePackage(GuidePackage package)
    {
        context.GuideTourPackages.Update(package);
    }

    public async Task<GuidePackage?> GetPackageByIdAsync(
        Guid packageId,
        bool isReadOnly,
        CancellationToken cancellationToken)
    {
        return await GetPackageByIdAsync(
            packageId,
            new IPackageRepository.PackageQueryOptions { ReadOnly = isReadOnly },
            cancellationToken);
    }
}