using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class GuideWeeklyScheduleRepository(ApplicationDbContext context) : IGuideWeeklyScheduleRepository
{
    public void AddSchedule(GuideWeeklySchedule schedule)
    {
        context.GuideWeeklySchedules.Add(schedule);
    }

    public async Task<GuideWeeklySchedule?> GetByGuideIdAndDayAsync(
    Guid guideId,
    DayOfWeek day,
    CancellationToken cancellationToken)
    {
        return await context.GuideWeeklySchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GuideId == guideId
                                    && x.DayOfWeek == day,
                                 cancellationToken);
    }
}
