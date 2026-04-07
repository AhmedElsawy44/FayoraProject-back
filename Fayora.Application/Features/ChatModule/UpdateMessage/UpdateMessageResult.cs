namespace Fayora.Application.Features.ChatModule.UpdateMessage;

public record UpdateMessageResult(
    Guid ReceiverId,
    long MessageId,
    Guid ChatId,
    string Content,
    DateTimeOffset UpdatedAt
);
