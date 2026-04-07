using Fayora.Domain.Entities.ChatModule;
using Fayora.Domain.Enums.ChatModule;

namespace Fayora.Application.Common.Interfaces.Presistances.ChatModule;

public interface IChatRepository
{
    void AddChat(Chat chat);
    Task<Chat?> GetByParticipantsAndScopeAsync(Guid senderId, Guid receiverId, ChatScopeType scopeType, Guid scopeId, CancellationToken cancellationToken);
    Task<Chat?> GetChatByIdAsync(Guid value, CancellationToken cancellationToken);
}
