using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Contracts.NotificationModule;
using Fayora.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.NotificationModule.Queries.GetUnreadCount;

public class GetUnreadCountQueryHandler(
    INotificationRepository notificationRepository,
    IClientContextProvider clientContextProvider)
    : IQueryHandler<GetUnreadCountQuery, Result<UnreadCountResponse>>
{
    public async Task<Result<UnreadCountResponse>> Handle(
        GetUnreadCountQuery request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var count = await notificationRepository.GetUnreadCountAsync(userId, cancellationToken);

        return new UnreadCountResponse(count);
    }
}
