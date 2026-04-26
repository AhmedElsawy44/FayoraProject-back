using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Domain.Entities.TouristModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories.TouristModule
{
    public class UserInteractionRepository(ApplicationDbContext context) : IUserInteractionRepository
    {
        public async Task AddInteractionAsync(
            UserInteraction interaction,
            CancellationToken cancellationToken = default)
        {
            await context.UserInteractions.AddAsync(interaction, cancellationToken);
        }
    }
}
