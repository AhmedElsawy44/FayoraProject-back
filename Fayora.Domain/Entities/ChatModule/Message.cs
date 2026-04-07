using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.ChatModule;
using Fayora.Domain.Errors;

namespace Fayora.Domain.Entities.ChatModule;

public class Message : AuditableEntity<long>
{
    public Guid ChatId { get; init; }
    public Guid SenderId { get; init; }
    public string Content { get; private set; } = default!;
    public MessageType Type { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    private Message(Guid chatId, Guid senderId, string content, MessageType type)
    {
        ChatId = chatId;
        SenderId = senderId;
        Content = content;
        Type = type;
    }

    public static Result<Message> Create(Guid chatId, Guid senderId, string content, MessageType type)
    {
        if (string.IsNullOrWhiteSpace(content))
            return ChatErrors.EmptyMessage;

        return new Message(chatId, senderId, content, type);
    }

    public void MarkAsRead()
    {
        ReadAt = DateTimeOffset.UtcNow;
    }

    public Result<Success> UpdateContent(string newContent)
    {
        if(CreatedAt + TimeSpan.FromHours(1) < DateTimeOffset.UtcNow)
        {
            return ChatErrors.MessageEditTimeExpired;
        }

        if (Type != MessageType.Text)
        {
            return ChatErrors.NonTextMessageCannotBeEdited;
        }

        if (string.IsNullOrWhiteSpace(newContent))
        {
            return ChatErrors.EmptyMessage;
        }

        Content = newContent;
        Updated();

        return Result.Success;
    }

    private Message() { }

}
