
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class MasterAmenityRepository(ApplicationDbContext context) : IMasterAmenityRepository
{
    public async Task<MasterAmenity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.MasterAmenities
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
    public async Task<List<MasterAmenity>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default)
    {
        return await context.Set<MasterAmenity>()
            .Where(a => ids.Contains(a.Id) && a.IsActive)
            .ToListAsync(cancellationToken);
    }
    public async Task<List<MasterAmenity>> GetAllAsync(bool IsActive = true, CancellationToken cancellationToken = default)
    {
        return await context.MasterAmenities
            .Where(a => a.IsActive == IsActive)
            .ToListAsync(cancellationToken);
    }

    public void Add(MasterAmenity amenity)
    {
        context.Set<MasterAmenity>().Add(amenity);
    }

    public void Delete(MasterAmenity amenity)
    {
        context.Set<MasterAmenity>().Remove(amenity);
    }
}