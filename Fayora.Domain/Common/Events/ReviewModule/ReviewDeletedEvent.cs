using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Enums.ReviewModule;

namespace Fayora.Domain.Common.Events.ReviewModule;

public record ReviewDeletedEvent(Guid ReviewId, Guid TargetId, ReviewTargetType TargetType, decimal Rating) : IDomainEvent;
