using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IGuideWeeklyScheduleRepository
{
    void AddSchedule(GuideWeeklySchedule schedule);

    Task<GuideWeeklySchedule?> GetByGuideIdAndDayAsync(
    Guid guideId,
    DayOfWeek day,
    CancellationToken cancellationToken);
}
