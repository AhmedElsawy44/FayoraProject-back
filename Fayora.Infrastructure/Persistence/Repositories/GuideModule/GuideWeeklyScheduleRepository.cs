using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Infrastructure.Persistence.Repositories.GuideModule;

public class GuideWeeklyScheduleRepository(ApplicationDbContext context) : IGuideWeeklyScheduleRepository
{
    public void AddSchedule(GuideWeeklySchedule schedule)
    {
        context.GuideWeeklySchedules.Add(schedule);
    }
}
