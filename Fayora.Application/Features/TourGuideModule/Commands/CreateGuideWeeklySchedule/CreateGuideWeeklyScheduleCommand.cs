using Fayora.Application.Common.Abstractions.Messaging;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuideWeeklySchedule;

public record CreateGuideWeeklyScheduleCommand(
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime
    ) : ICommand<Unit>;
