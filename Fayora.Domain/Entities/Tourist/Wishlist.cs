namespace Fayora.Domain.Entitties.Tourist;

public class Wishlist : BaseEntity<Guid>
{
    public Guid TouristId { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public Guid ItemId { get; init; } = Guid.Empty;
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.UtcNow;

    public Wishlist(Guid touristId, string itemType, Guid itemId)
    {
        Id = Guid.CreateVersion7();
        TouristId = touristId;
        ItemType = itemType;
        ItemId = itemId;
    }

    private Wishlist() { }
}
