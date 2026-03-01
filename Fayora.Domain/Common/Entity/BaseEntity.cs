public abstract class BaseEntity<TKey> : HasDomainEvents
{
    public TKey Id { get; init; } = default!;
}
