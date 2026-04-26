using Fayora.Domain.Entities.TouristModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.TouristModule
{
    public interface IUserInteractionRepository
    {
        Task AddInteractionAsync(UserInteraction interaction, CancellationToken cancellationToken = default);
    }
}
