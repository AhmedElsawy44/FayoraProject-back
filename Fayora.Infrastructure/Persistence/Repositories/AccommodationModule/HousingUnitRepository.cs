using Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class HousingUnitRepository(ApplicationDbContext context) : IHousingUnitRepository
{
    public void AddUnit(HousingUnit housingUnit)
    {
        context.HousingUnits.Add(housingUnit);
    }
}
