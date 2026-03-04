using Fayora.Domain.Common.Interfaces;

namespace Fayora.Domain.Common.Events;

public record UserVerifiedDomainEvent(Guid UserId, string Target) : IDomainEvent;
