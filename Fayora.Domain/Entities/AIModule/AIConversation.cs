using Fayora.Domain.Common.Entity;

namespace Fayora.Domain.Entities.AIModule;

public class AIConversation : AuditableEntity<Guid>
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;

    public AIConversation(Guid userId, string title)
    {
        UserId = userId;
        Title = title;
    }
}
