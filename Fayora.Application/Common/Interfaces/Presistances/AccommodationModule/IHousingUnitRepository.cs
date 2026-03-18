using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Presistances.AccommodationModule;

public interface IHousingUnitRepository
{
    void AddUnit(HousingUnit housingUnit);
}
