using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.ChatModule;
using Fayora.Domain.Errors;

namespace Fayora.Domain.Entities.ChatModule;

public class Chat : AuditableEntity<Guid>
{
    public Guid FirstUserId { get; private set; }
    public Guid SecondUserId { get; private set; }
    public ChatScopeType ScopeType { get; private set; }
    public Guid ScopeId { get; private set; }

    private readonly List<Message> _messages = [];
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();



    private Chat(Guid firstUserId, Guid secondUserId, ChatScopeType scopeType, Guid scopeId)
    {
        Id = Guid.NewGuid();
        FirstUserId = firstUserId;
        SecondUserId = secondUserId;
        ScopeType = scopeType;
        ScopeId = scopeId;
    }

    public static Result<Chat> Create(Guid firstUserId, Guid secondUserId, ChatScopeType scopeType, Guid scopeId)
    {
        if (firstUserId == secondUserId)
            return ChatErrors.SelfChatNotAllowed;

        return new Chat(firstUserId, secondUserId, scopeType, scopeId);
    }

    public void UpdateLastMessageAt()
    {
        Updated();
    }

    private Chat() { }
}
