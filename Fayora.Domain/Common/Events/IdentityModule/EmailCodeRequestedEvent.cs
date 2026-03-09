using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Shared.IdentityModule;

namespace Fayora.Domain.Common.Events.IdentityModule;

public record EmailCodeRequestedEvent(Guid UserId,
string Email,
string Code,
CodePurpose Purpose) : IDomainEvent;
