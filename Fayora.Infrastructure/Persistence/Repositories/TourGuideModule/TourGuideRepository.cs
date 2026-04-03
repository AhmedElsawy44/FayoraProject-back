using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Infrastructure.Persistence.Repositories.TourGuideModule;

public class TourGuideRepository(ApplicationDbContext context) : ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide)
    {
        context.TourGuides.Add(tourGuide);
    }
}
