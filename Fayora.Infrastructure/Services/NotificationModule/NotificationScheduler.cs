using Fayora.Application.Common.Interfaces.Services.NotificationModule;
using Fayora.Infrastructure.Jobs;
using Hangfire;

namespace Fayora.Infrastructure.Services.NotificationModule;

public class NotificationScheduler(
    IBackgroundJobClient backgroundJobClient,
    IRecurringJobManager recurringJobManager) : INotificationScheduler
{
    public string ScheduleCampaign(Guid campaignId, DateTimeOffset scheduledAt)
    {
        return backgroundJobClient.Schedule<PushCampaignJob>(
            job => job.ExecuteAsync(campaignId, CancellationToken.None),
            scheduledAt);
    }

    public string EnqueueCampaign(Guid campaignId)
    {
        return backgroundJobClient.Enqueue<PushCampaignJob>(
            job => job.ExecuteAsync(campaignId, CancellationToken.None));
    }

    public bool CancelCampaign(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId))
        {
            return false;
        }
        return backgroundJobClient.Delete(jobId);
    }

    public void ScheduleRecurringCampaign(Guid campaignId, string cronExpression)
    {
        recurringJobManager.AddOrUpdate<PushCampaignJob>(
            $"campaign-{campaignId}",
            job => job.ExecuteAsync(campaignId, CancellationToken.None),
            cronExpression,
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
    }

    public void CancelRecurringCampaign(Guid campaignId)
    {
        recurringJobManager.RemoveIfExists($"campaign-{campaignId}");
    }
}
