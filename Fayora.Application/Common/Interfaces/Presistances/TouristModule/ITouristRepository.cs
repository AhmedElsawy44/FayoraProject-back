using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Common.Interfaces.Presistances.TouristModule;

public interface ITouristRepository
{
    public void AddTourist(TouristProfile touristProfile);
}
