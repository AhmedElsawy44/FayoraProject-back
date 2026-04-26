using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Infrastructure.Persistence.Repositories.TouristModule;

public class UserInteractionRepository(ApplicationDbContext context) : IUserInteractionRepository
{
    public void AddInteraction(UserInteraction interaction)
    {
        context.UserInteractions.Add(interaction);
    }
}
