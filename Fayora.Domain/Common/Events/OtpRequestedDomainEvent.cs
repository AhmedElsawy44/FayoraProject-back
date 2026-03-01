using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Common.Events;

public record OtpRequestedDomainEvent(Guid UserId,
    string Target,
    string Code,
    OtpPurpose Purpose) : IDomainEvent;
