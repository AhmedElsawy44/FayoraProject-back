using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class PackageOccurrenceRepository(ApplicationDbContext context)
    : IPackageOccurrenceRepository
{
    public async Task AddRangeAsync(List<PackageOccurrence> occurrences, CancellationToken cancellationToken)
    {
        await context.PackageOccurrences.AddRangeAsync(occurrences, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<PackageOccurrence?> GetOccurrenceByPackageIdAndDate(Guid packageId, DateOnly date, CancellationToken cancellationToken)
    {
        return context.PackageOccurrences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PackageId == packageId && x.Date == date, cancellationToken);
    }

    public async Task<bool> HasOverlappingOccurrenceAsync(Guid packageId, List<DateOnly> dates, CancellationToken cancellationToken)
    {
        return await context.PackageOccurrences
            .AnyAsync(x => x.PackageId == packageId
                        && dates.Contains(x.Date)
                        && x.Status != OccurrenceStatus.Cancelled,
                      cancellationToken);
    }

    public async Task ReleaseSeatsAsync(
        Guid packageId,
        DateOnly date,
        int count,
        CancellationToken cancellationToken)
    {
        var occurrence = await context.PackageOccurrences
            .FirstOrDefaultAsync(x => x.PackageId == packageId
                                 && x.Date == date,
                                 cancellationToken);

        occurrence?.ReleaseSeats(count);
    }
}
