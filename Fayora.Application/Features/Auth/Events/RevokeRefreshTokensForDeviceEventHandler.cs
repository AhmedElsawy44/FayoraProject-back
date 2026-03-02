using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Common.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Events;

public class RevokeRefreshTokensForDeviceEventHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<RevokeRefreshTokensForDeviceEventHandler> logger
    ) : INotificationHandler<RevokeRefreshTokensForDeviceEvent>
{
    public async Task Handle(RevokeRefreshTokensForDeviceEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await refreshTokenRepository.RevokeTokensForDeviceAsync(
                notification.UserId,
                notification.DeviceId,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error occurred while revoking refresh tokens for User: {UserId} on Device: {DeviceId}",
                notification.UserId,
                notification.DeviceId);
            throw;
        }
    }
}