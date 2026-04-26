using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Common.Interfaces.Persistences.TouristModule;

public interface IUserInteractionRepository
{
    void AddInteraction(UserInteraction interaction);
}
