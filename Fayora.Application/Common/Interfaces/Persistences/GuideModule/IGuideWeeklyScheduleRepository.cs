using Fayora.Domain.Entities.GuideModule;

namespace Fayora.Application.Common.Interfaces.Persistences.GuideModule;

public interface IGuideWeeklyScheduleRepository
{
    void AddSchedule(GuideWeeklySchedule schedule);
}
