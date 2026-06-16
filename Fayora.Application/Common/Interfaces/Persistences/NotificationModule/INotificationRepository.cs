using Fayora.Contracts.AdminModule.Notifications;
using Fayora.Domain.Entities.NotificationModule;

namespace Fayora.Application.Common.Interfaces.Persistences.NotificationModule;

public interface INotificationRepository
{
    Task AddDeviceTokenAsync(DeviceToken token, CancellationToken ct);
    Task<DeviceToken?> GetDeviceTokenByTokenAsync(string token, CancellationToken ct);
    Task<List<string>> GetTokensByAudienceAsync(string targetAudience, CancellationToken ct);
    Task AddPushCampaignAsync(PushCampaign campaign, CancellationToken ct);
    Task<PushCampaign?> GetPushCampaignByIdAsync(Guid id, CancellationToken ct);
    Task<List<PushCampaign>> GetPushCampaignsPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<PushCampaignStatsResponse> GetPushCampaignStatsAsync(CancellationToken ct);

    // In-App Notifications
    Task AddInAppNotificationAsync(InAppNotification notification, CancellationToken ct);
    Task<List<InAppNotification>> GetInAppNotificationsPaginatedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken ct);
    Task<InAppNotification?> GetInAppNotificationByIdAsync(Guid id, Guid userId, CancellationToken ct);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
    Task<List<string>> GetTokensByUserIdAsync(Guid userId, CancellationToken ct);
    Task<List<InAppNotification>> GetUnreadNotificationsByUserIdAsync(Guid userId, CancellationToken ct);
}
