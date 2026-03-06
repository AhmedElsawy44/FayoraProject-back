using Fayora.Domain.Common.Interfaces;

namespace Fayora.Domain.Common.Events;

public record UserSecurityActivityDomainEvent(
    Guid UserId,
    string Target,
    SecurityActivityType ActivityType
) : IDomainEvent;

public enum SecurityActivityType
{
    EmailVerified = 1,
    PasswordReset = 2,
    EmailChanged = 3,
    AccountLocked = 4,
    AccountRestored = 5,
    AccountDeleted = 6,
}