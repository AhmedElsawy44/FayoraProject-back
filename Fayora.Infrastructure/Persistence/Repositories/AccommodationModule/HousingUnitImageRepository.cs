using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Infrastructure.Persistence.Repositories.AccommodationModule;

public class HousingUnitImageRepository(ApplicationDbContext context) : IHousingUnitImageRepository
{
    public void AddImages(IEnumerable<HousingUnitImage> images)
    {
        context.HousingUnitImages.AddRange(images);
    }
}
