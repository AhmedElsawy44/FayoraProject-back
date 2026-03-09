using Fayora.Domain.Common.Interfaces.IdentityModule;

namespace Fayora.Domain.Common.Events.IdentityModule;

public record DeleteMediaEvent(string MediaURL) : IDomainEvent;