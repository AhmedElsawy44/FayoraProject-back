using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule
{
    public class PackageNightRepository(ApplicationDbContext context) : IPackageNightRepository
    {
        public void Add(PackageNight night)
            => context.PackageNights.Add(night);

        public async Task<List<PackageNight>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
            => await context.PackageNights
                .AsNoTracking()
                .Where(n => n.PackageId == packageId)
                .OrderBy(n => n.NightNumber)
                .ToListAsync(cancellationToken);
    }
}
