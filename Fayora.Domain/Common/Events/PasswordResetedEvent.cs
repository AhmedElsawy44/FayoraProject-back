using Fayora.Domain.Common.Interfaces;

namespace Fayora.Domain.Common.Events;

public record PasswordResetedEvent(Guid UserId, string Email) : IDomainEvent;
