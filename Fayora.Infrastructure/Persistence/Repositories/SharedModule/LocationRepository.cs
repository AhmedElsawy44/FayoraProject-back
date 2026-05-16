using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.Enums.SharedModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.SharedModule
{

    public class LocationRepository(ApplicationDbContext context) : ILocationRepository
    {
        public void AddLocation(Location location) => context.Locations.Add(location);

        public async Task<(List<Location> Items, int TotalCount)> GetAllLocationsAsync(
            LocationCategory? category,
            decimal? minRating,
            string? search,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = context.Locations.AsNoTracking();

            if (category.HasValue)
                query = query.Where(l => l.Category == category.Value);

            if (minRating.HasValue)
                query = query.Where(l => l.Rating >= minRating.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(l => l.Name.Contains(search));

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(l => Guid.NewGuid())
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<Location?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default)
            => await context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        public async Task<bool> LocationExistsAsync(int id, CancellationToken cancellationToken = default)
            => await context.Locations.AnyAsync(l => l.Id == id, cancellationToken);

        public void RemoveLocation(Location location) => context.Locations.Remove(location);

    }
}
