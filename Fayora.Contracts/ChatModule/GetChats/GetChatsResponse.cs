namespace Fayora.Contracts.ChatModule.GetChats;

public record GetChatsResponse(
    IEnumerable<Chat> Chats
);

public record Chat(
    Guid ChatId,
    Guid OtherUserId,
    string? OtherUserName,
    string? OtherUserAvatarUrl,
    string? LastMessageSnippet,
    DateTimeOffset? LastMessageTimestamp,
    int UnreadCount
);
