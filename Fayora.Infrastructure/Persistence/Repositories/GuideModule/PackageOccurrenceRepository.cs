using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule
{
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

        public async Task<bool> PackageExistsForUserAsync(Guid packageId, Guid userId, CancellationToken cancellationToken)
        {
            return await context.GuideTourPackages
                .AnyAsync(x => x.Id == packageId && x.UserId == userId, cancellationToken);
        }
    }
}
