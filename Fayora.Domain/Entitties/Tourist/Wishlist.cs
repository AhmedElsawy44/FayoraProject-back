namespace Fayora.Domain.Entitties.Tourist;

public class Wishlist : BaseEntity<Guid>
{
    public Guid TourstId { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public Guid ItemId { get; init; } = Guid.Empty;
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.UtcNow;

    private Wishlist() { }
}
