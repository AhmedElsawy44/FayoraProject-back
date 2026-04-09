namespace Fayora.Contracts.ChatModule.GetMessages;

public record GetMessagesResponse(
    IEnumerable<Message> Messages
);

public record Message(
    Guid Id,
    Guid ChatId,
    Guid SenderId,
    string Content,
    DateTimeOffset SentAt,
    DateTimeOffset? ReadAt
);
