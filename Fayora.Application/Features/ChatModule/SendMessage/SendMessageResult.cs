namespace Fayora.Application.Features.ChatModule.SendMessage;

public record SendMessageResult(
    long MessageId,
    Guid ChatId,
    Guid SenderId,
    string SenderName,
    string? SenderAvatarUrl
);
