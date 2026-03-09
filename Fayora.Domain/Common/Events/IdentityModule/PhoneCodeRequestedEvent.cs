using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Shared.IdentityModule;

namespace Fayora.Domain.Common.Events.IdentityModule;

public record PhoneCodeRequestedEvent(Guid UserId,
    string PhoneNumber,
    string Code,
    CodePurpose Purpose,
    CodeDeliveryMethod DeliveryMethod = CodeDeliveryMethod.WhatsApp) : IDomainEvent;
