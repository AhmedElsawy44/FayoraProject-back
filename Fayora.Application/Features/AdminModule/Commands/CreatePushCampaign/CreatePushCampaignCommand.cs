using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.CreatePushCampaign;

public record CreatePushCampaignCommand(
    string Title,
    string Body,
    string? ImageUrl,
    string TargetAudience,
    DateTimeOffset? ScheduledAt,
    bool IsRecurring = false,
    string? ScheduleType = null,
    string? DaysOfWeek = null,
    int? DayOfMonth = null,
    TimeSpan? PreferredTime = null,
    string? CronExpression = null) : ICommand<Result<Guid>>;
