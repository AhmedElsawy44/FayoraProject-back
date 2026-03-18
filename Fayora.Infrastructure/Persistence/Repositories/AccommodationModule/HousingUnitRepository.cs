using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class HousingUnitRepository(ApplicationDbContext context) : IHousingUnitRepository
{
    public void AddUnit(HousingUnit housingUnit)
    {
        context.HousingUnits.Add(housingUnit);
    }

    public async Task<HousingUnit?> GetUnitByIdAsync(
    Guid unitId,
    IHousingUnitRepository.UnitQueryOptions? options = null,
    CancellationToken cancellationToken = default)
    {
        IQueryable<HousingUnit> query = context.HousingUnits;

        if (options is not null)
        {
            if (options.IsReadOnly)
            {
                query = query.AsNoTracking();
            }

            if (options.IncludeAmenities)
            {
                query = query.Include(u => u.Amenities);
            }

            if (options.IncludeImages)
            {
                query = query.Include(u => u.Images);
            }
        }

        return await query.FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);
    }
}
