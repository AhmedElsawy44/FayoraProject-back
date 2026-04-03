using Fayora.Domain.Entities.TourGuide;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuideRepository
{
    public void AddTourGuide(TourGuide tourGuide);
}
