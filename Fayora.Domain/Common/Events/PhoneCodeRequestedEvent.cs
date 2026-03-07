using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Common.Events;

public record PhoneCodeRequestedEvent(Guid UserId,
    string PhoneNumber,
    string Code,
    CodePurpose Purpose,
    CodeDeliveryMethod DeliveryMethod = CodeDeliveryMethod.WhatsApp) : IDomainEvent;
