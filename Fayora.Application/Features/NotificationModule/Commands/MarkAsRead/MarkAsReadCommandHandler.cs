using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Fayora.Application.Features.NotificationModule.Commands.MarkAsRead;

public class MarkAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider)
    : ICommandHandler<MarkAsReadCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        MarkAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var notification = await notificationRepository.GetInAppNotificationByIdAsync(
            request.NotificationId,
            userId,
            cancellationToken);

        if (notification is null)
        {
            return Error.NotFound("Notification.NotFound", "Notification not found.");
        }

        notification.MarkAsRead();

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}
