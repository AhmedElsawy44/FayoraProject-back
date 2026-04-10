using Fayora.Domain.Enums.AIModule;

namespace Fayora.Domain.Entities.AIModule;

public class AIMessage : BaseEntity<Guid>
{
    public Guid ConversationId { get; init; }
    public Sender Sender { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.UtcNow;

    private readonly List<AIMessageRecommendation> _recommendations = [];
    public IReadOnlyList<AIMessageRecommendation> Recommendations => _recommendations.AsReadOnly();

    public AIMessage(Guid conversationId, Sender sender, string content)
    {
        ConversationId = conversationId;
        Sender = sender;
        Content = content;
    }

    public void AddRecommendations(List<AIMessageRecommendation> recommendations)
    {
        _recommendations.AddRange(recommendations);
    }
}
