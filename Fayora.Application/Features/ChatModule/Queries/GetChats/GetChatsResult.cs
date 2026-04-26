namespace Fayora.Application.Features.ChatModule.Queries.GetChats;

public record GetChatsResult(IEnumerable<ChatDto> Chats);

public record ChatDto(
    Guid ChatId,
    Guid OtherUserId,
    string? OtherUserName,
    string? OtherUserAvatarUrl,
    string? LastMessageSnippet,
    DateTimeOffset? LastMessageTimestamp,
    int UnreadCount);
