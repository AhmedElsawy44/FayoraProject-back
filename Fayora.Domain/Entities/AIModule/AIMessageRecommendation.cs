using Fayora.Domain.Enums.AIModule;

namespace Fayora.Domain.Entities.AIModule;

public class AIMessageRecommendation : BaseEntity<Guid>
{
    public Guid MessageId { get; init; }
    public RecommendationType Type { get; init; }
    public Guid ItemId { get; init; }

    internal AIMessageRecommendation(Guid messageId, RecommendationType type, Guid itemId)
    {
        MessageId = messageId;
        Type = type;
        ItemId = itemId;
    }
}
