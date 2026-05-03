namespace Fayora.Domain.Entities.GuideModule;

public class GuideWeeklySchedule : BaseEntity<Guid>
{
    public Guid GuideId { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public GuideWeeklySchedule(Guid guideId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        Id = Guid.CreateVersion7();
        GuideId = guideId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
}
