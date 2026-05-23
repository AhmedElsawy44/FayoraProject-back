using System;

namespace Fayora.Contracts.NotificationModule;

public record InAppNotificationResponse(
    Guid Id,
    string Title,
    string Body,
    bool IsRead,
    string? Type,
    string? EntityId,
    DateTimeOffset CreatedAt
);

public record GetNotificationsQueryRequest(
    int PageNumber = 1,
    int PageSize = 10
);

public record UnreadCountResponse(
    int Count
);
