namespace Fayora.Contracts.ChatModule.GetChats;

public record GetChatsResponse(
    IEnumerable<ChatDto> Chats
);

public record ChatDto(
    Guid ChatId,
    Guid OtherUserId,
    string? OtherUserName,
    string? OtherUserAvatarUrl,
    string? LastMessageSnippet,
    DateTimeOffset? LastMessageTimestamp,
    int UnreadCount
);
