using Fayora.Domain.Entities.ChatModule;
using Fayora.Domain.Enums.ChatModule;

namespace Fayora.Application.Common.Interfaces.Presistances.ChatModule;

public interface IChatRepository
{
    void AddChat(Chat chat);
    Task<Chat?> GetByParticipantsAndScopeAsync(Guid senderId, Guid receiverId, ChatScopeType scopeType, Guid scopeId, CancellationToken cancellationToken);
    Task<Chat?> GetChatByIdAsync(Guid chatId, bool IsReadOnly, CancellationToken cancellationToken);
    Task<List<ChatItemDto>> GetUserChatsPagedAsync(
        Guid userId,
        int limit,
        DateTimeOffset? cursor,
        CancellationToken cancellationToken = default);

    public record ChatItemDto
    (
        Guid ChatId,
        Guid OtherUserId,
        string OtherUserName,
        string? OtherUserAvatarUrl,
        string? LastMessageContent,
        DateTimeOffset? LastMessageTime,
        int UnreadCount
    );
}
