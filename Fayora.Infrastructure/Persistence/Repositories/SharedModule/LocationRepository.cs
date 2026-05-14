using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Entities.SharedModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Fayora.Infrastructure.Persistence.Repositories.SharedModule.LocationRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule
{

    public class LocationRepository(ApplicationDbContext context) : ILocationRepository
    {
        public void AddLocation(Location location) => context.Locations.Add(location);

        public async Task<Location?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default)
            => await context.Locations.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        public async Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken = default)
            => await context.Locations.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<bool> LocationExistsAsync(int id, CancellationToken cancellationToken = default)
            => await context.Locations.AnyAsync(l => l.Id == id, cancellationToken);

        public void RemoveLocation(Location location) => context.Locations.Remove(location);

    }
}
