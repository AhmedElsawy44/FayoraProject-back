using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;

public interface IHousingUnitImageRepository
{
    public void AddImages(IEnumerable<HousingUnitImage> images);
}
