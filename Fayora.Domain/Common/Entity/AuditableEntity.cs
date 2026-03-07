namespace Fayora.Domain.Common.Entity;

public abstract class AuditableEntity<TKey> : BaseEntity<TKey>
{
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    protected AuditableEntity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    protected void Updated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}