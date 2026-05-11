using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class PackageRepository(ApplicationDbContext context) : IPackageRepository
{
    public void AddPackage(GuidePackage package)
    {
        context.GuideTourPackages.Add(package);
    }

    public async Task<List<PackageActivity>> GetActivitiesByPackageIdAsync(
        Guid packageId,
        CancellationToken cancellationToken = default)
    {
        return await context.PackageActivities
            .AsNoTracking()
            .Where(a => a.PackageId == packageId)
            .ToListAsync(cancellationToken);
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

    public async Task<GuidePackage?> GetPackageWithOccurrencesAsync(
        Guid packageId,
        CancellationToken cancellationToken = default)
    {
        return await context.GuideTourPackages
            .AsNoTracking()
            .Include(p => p.Occurrences.Where(o =>
                o.Date >= DateOnly.FromDateTime(DateTime.UtcNow) &&
                o.AvailableSeats > 0))
            .FirstOrDefaultAsync(p => p.Id == packageId && p.Status == ItemStatus.Active, cancellationToken);
    }
}