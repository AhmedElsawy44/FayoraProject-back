using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Common.Events;

public record EmailCodeRequestedEvent(Guid UserId,
string Email,
string Code,
CodePurpose Purpose) : IDomainEvent;
