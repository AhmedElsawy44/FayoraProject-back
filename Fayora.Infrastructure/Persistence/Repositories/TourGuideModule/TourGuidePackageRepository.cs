using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;

public class TourGuidePackageRepository(ApplicationDbContext context) : ITourGuidePackageRepository
{
    public void AddPackageAsync(GuideTourPackage package, CancellationToken cancellationToken)
    {
        context.GuideTourPackages.Add(package);
    }

    public async Task<GuideTourPackage?> GetPackageByIdAsync(
    Guid packageId,
    bool IsReadOnly,
    CancellationToken cancellationToken)
    {
        IQueryable<GuideTourPackage> query = context.GuideTourPackages;

        if (IsReadOnly)
        {
            query = query.AsNoTracking();
        }

        return await query
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);
    }

    public void DeletePackage(GuideTourPackage package)
    {
        context.GuideTourPackages.Remove(package);
    }
}
