using Fayora.Domain.Common.Interfaces;

namespace Fayora.Domain.Common.Events;

public record DeleteMediaEvent(string MediaURL) : IDomainEvent;