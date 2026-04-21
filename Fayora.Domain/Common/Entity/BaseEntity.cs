public abstract class BaseEntity<TKey> : AggregateRoot
{
    public TKey Id { get; init; } = default!;
}
