using Fayora.Application.Common.Interfaces.Presistances.TouristModule;
using Fayora.Domain.Entitties.Tourist;
using Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

namespace Fayora.Infrastructure.Persistence.Repositories.TouristModule;

public class TouristRepository(ApplicationDbContext context) : ITouristRepository
{
    public void AddTourist(TouristProfile touristProfile)
    {
        context.Tourists.Add(touristProfile);
    }
}
