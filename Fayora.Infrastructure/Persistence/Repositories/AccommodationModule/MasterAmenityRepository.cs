using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class MasterAmenityRepository(ApplicationDbContext context) : IMasterAmenityRepository
{
    public  Task<List<MasterAmenity>> GetAllAmenitiesAsync(CancellationToken cancellationToken)
    {
        return context.MasterAmenities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MasterAmenity>> GetAmenitiesByIdsAsync(
    List<int> amenityIds,
    CancellationToken cancellationToken = default)
    {
        if (amenityIds == null || amenityIds.Count == 0)
        {
            return new List<MasterAmenity>();
        }

        return await context.MasterAmenities
            .AsNoTracking()
            .Where(a => amenityIds.Contains(a.Id))
            .ToListAsync(cancellationToken);
    }
}
