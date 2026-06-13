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

    public async Task<List<GuidePackage>> GetListByIdsAsync(
    List<Guid> packageIds,
    CancellationToken cancellationToken)
    {
        if (packageIds == null || packageIds.Count == 0)
        {
            return [];
        }

        return await context.GuideTourPackages
            .AsNoTracking()
            .Where(package => packageIds.Contains(package.Id))
            .ToListAsync(cancellationToken);
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

    public void AddPackageActivities(IEnumerable<PackageActivity> activities)
    {
        context.PackageActivities.AddRange(activities);
    }

    public void RemovePackageActivities(IEnumerable<PackageActivity> activities)
    {
        context.PackageActivities.RemoveRange(activities);
    }

    public async Task<(List<GuidePackage> Items, int TotalCount)> GetMyPackagesAsync(
        Guid userId,
        ItemStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.DeletedAt == null);

        if (status.HasValue)
            query = query.Where(p => p.PackageStatus == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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

        if (options.IncludeOccurrences)
        {
            query = query.Include(p => p.Occurrences);
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
            .FirstOrDefaultAsync(p => p.Id == packageId && p.PackageStatus == ItemStatus.Active, cancellationToken);
    }


    public async Task<(List<GuidePackage> Items, int TotalCount)> GetActivePackagesAsync(
    string? search,
    int? locationId,
    ProviderType? providerType,
    ItemStatus? tourType,
    int? minDuration,
    int? maxDuration,
    decimal? minPrice,
    decimal? maxPrice,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        var query = context.GuideTourPackages
            .AsNoTracking()
            .Where(p => p.PackageStatus == ItemStatus.Active && p.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Title.Contains(search) ||
                p.Description.Contains(search));

        if (providerType.HasValue)
            query = query.Where(p => p.ProviderType == providerType.Value);

        if (minDuration.HasValue)
            query = query.Where(p => p.DurationHours >= minDuration.Value);

        if (maxDuration.HasValue)
            query = query.Where(p => p.DurationHours <= maxDuration.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.AdultPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.AdultPrice <= maxPrice.Value);


        var allPackages = await query.ToListAsync(cancellationToken);


        if (locationId.HasValue)
            allPackages = allPackages
                .Where(p => p.LocationIds.Contains(locationId.Value))
                .ToList();


        if (!string.IsNullOrWhiteSpace(search))
            allPackages = allPackages
                .Where(p => p.LocationIds.Any(id =>
                    context.Locations
                        .Any(l => l.Id == id && l.Name.Contains(search))))
                .ToList();

        var totalCount = allPackages.Count;

        var items = allPackages
            .OrderByDescending(p => p.Views)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }
}