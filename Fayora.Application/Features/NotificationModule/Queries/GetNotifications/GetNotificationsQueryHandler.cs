using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.NotificationModule;
using Fayora.Domain.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.NotificationModule.Queries.GetNotifications;

public class GetNotificationsQueryHandler(
    INotificationRepository notificationRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetNotificationsQuery, Result<List<InAppNotificationResponse>>>
{
    public async Task<Result<List<InAppNotificationResponse>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var notifications = await notificationRepository.GetInAppNotificationsPaginatedAsync(
            userId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var response = notifications.Select(n => new InAppNotificationResponse(
            n.Id,
            n.Title,
            n.Body,
            n.IsRead,
            n.Type,
            n.EntityId,
            n.CreatedAt
        )).ToList();

        return response;
    }
}
