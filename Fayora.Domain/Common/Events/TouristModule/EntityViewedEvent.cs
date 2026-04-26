using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Enums.TouristModule;

namespace Fayora.Domain.Common.Events.TouristModule
{
    public record EntityViewedEvent(
        Guid EntityId,
        EntityType EntityType
    ) : IDomainEvent;
}
