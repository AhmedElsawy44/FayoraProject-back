namespace Fayora.Application.Features.ChatModule.Commands.SendMessage;

public record SendMessageResult(
    long MessageId,
    Guid ChatId,
    Guid SenderId,
    string SenderName,
    string? SenderAvatarUrl,
    DateTimeOffset SentAt
);
