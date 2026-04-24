using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
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
        }

        return await query.FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);
    }
}
