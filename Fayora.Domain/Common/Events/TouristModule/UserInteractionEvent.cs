using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Entities.TouristModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Common.Events.TouristModule
{
    public record UserInteractionEvent
    (
        Guid UserId,
        Guid EntityId,
        EntityType EntityType,
        InteractionType InteractionType
    ) : IDomainEvent;
}
