namespace Fayora.Contracts.TourGuideModule.CreateWeeklySchedule;

public record CreateWeeklyScheduleRequest
(
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime
);
