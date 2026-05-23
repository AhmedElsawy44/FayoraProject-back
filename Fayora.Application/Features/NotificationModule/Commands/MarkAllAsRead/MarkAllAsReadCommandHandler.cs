using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.NotificationModule.Commands.MarkAllAsRead;

public class MarkAllAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider)
    : ICommandHandler<MarkAllAsReadCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        MarkAllAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var unreadNotifications = await notificationRepository.GetUnreadNotificationsByUserIdAsync(
            userId,
            cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
