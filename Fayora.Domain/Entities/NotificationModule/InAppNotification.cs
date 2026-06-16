using System;
using Fayora.Domain.Common.Entity;

namespace Fayora.Domain.Entities.NotificationModule;

public class InAppNotification : AuditableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public string? Type { get; private set; } // e.g. "booking_details", "chat_message", "admin_campaign"
    public string? EntityId { get; private set; } // e.g. bookingId, chatId, campaignId

    private InAppNotification() { }

    public static InAppNotification Create(Guid userId, string title, string body, string? type = null, string? entityId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be null or empty.", nameof(body));

        return new InAppNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Body = body,
            IsRead = false,
            Type = type,
            EntityId = entityId
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            Updated();
        }
    }
}
