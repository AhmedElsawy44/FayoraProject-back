using Fayora.Domain.Enums.TouristModule;

namespace Fayora.Domain.Entities.TouristModule;

public class UserInteraction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public InteractionType InteractionType { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserInteraction() { }

    public UserInteraction(
        Guid userId,
        Guid entityId,
        EntityType entityType,
        InteractionType interactionType)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        EntityId = entityId;
        EntityType = entityType;
        InteractionType = interactionType;
        CreatedAt = DateTime.UtcNow;
    }
}