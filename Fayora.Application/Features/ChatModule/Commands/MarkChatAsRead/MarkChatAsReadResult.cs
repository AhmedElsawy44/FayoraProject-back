namespace Fayora.Application.Features.ChatModule.Commands.MarkChatAsRead;

public record MarkChatAsReadResult(
    Guid ReceiverId,
    Guid ChatId,
    DateTimeOffset ReadAt);
