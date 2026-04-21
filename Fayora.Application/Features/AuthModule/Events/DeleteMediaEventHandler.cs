using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Domain.Common.Events.IdentityModule;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.AuthModule.Events;

public class DeleteMediaEventHandler(
    IFileStorageService fileStorageService,
    ILogger<DeleteMediaEventHandler> logger)
    : INotificationHandler<DeleteMediaEvent>
{
    public async Task Handle(DeleteMediaEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.MediaURL))
        {
            logger.LogWarning("DeleteMediaEvent triggered with an empty MediaURL.");
            return;
        }

        try
        {
            await fileStorageService.DeleteFileAsync(notification.MediaURL, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete media: {MediaUrl}. Reason: {Message}", notification.MediaURL, ex.Message);
        }
    }
}