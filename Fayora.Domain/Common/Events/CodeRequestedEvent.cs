using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Common.Events;

public record CodeRequestedEvent(Guid UserId,
    string Target,
    string Code,
    CodePurpose Purpose,
    CodeDeliveryMethod DeliveryMethod) : IDomainEvent;
