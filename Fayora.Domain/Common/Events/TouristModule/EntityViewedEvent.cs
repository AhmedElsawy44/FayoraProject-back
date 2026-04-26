using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Enums.TouristModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Common.Events.TouristModule
{
    public record EntityViewedEvent(
        Guid EntityId,
        EntityType EntityType
    ) : IDomainEvent;
}
