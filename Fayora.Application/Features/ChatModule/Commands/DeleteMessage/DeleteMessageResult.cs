namespace Fayora.Application.Features.ChatModule.Commands.DeleteMessage;

public record DeleteMessageResult(
    Guid ReceiverId,
    long MessageId,
    Guid ChatId,
    DateTimeOffset DeletedAt);
