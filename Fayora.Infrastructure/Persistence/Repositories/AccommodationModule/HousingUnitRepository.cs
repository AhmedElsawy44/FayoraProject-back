using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.Enums.TourGuideModule;
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

    public async Task<List<HousingUnit>> GetUnitsByTypeAsync(
        HousingType type,
        CancellationToken cancellationToken = default)
    {
        return await context.HousingUnits
            .AsNoTracking()
            .Where(u => u.Type == type && u.Status == ItemStatus.Active)
            .OrderBy(u => Guid.NewGuid())
            .ToListAsync(cancellationToken);
    }
}
