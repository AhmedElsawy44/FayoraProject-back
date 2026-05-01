namespace Fayora.Application.Features.ChatModule.Commands.UpdateMessage;

public record UpdateMessageResult(
    Guid ReceiverId,
    long MessageId,
    Guid ChatId,
    string Content,
    DateTimeOffset UpdatedAt
);
