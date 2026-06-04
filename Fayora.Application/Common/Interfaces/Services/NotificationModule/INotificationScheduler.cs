namespace Fayora.Application.Common.Interfaces.Services.NotificationModule;

public interface INotificationScheduler
{
    string ScheduleCampaign(Guid campaignId, DateTimeOffset scheduledAt);
    string EnqueueCampaign(Guid campaignId);
    bool CancelCampaign(string jobId);
    void ScheduleRecurringCampaign(Guid campaignId, string cronExpression);
    void CancelRecurringCampaign(Guid campaignId);
}
