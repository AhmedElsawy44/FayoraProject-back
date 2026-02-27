using Fayora.Domain.Common.Interfaces;

public abstract class BaseEntity<TKey> : HasDomainEvents
{
    public TKey Id { get; init; } = default!;
}
