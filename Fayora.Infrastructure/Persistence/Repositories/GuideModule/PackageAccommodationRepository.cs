using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule
{
    public class PackageAccommodationRepository(ApplicationDbContext context) : IPackageAccommodationRepository
    {
        public void Add(PackageAccommodation accommodation)
            => context.PackageAccommodations.Add(accommodation);

        public async Task<PackageAccommodation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await context.PackageAccommodations
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        public async Task<List<PackageAccommodation>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
            => await context.PackageAccommodations
                .Where(a => a.PackageId == packageId)
                .ToListAsync(cancellationToken);

        public void RemoveAccommodations(List<PackageAccommodation> accommodations)
            => context.PackageAccommodations.RemoveRange(accommodations);
    }
}
