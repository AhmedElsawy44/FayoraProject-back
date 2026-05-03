using FluentValidation;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuideWeeklySchedule;

public class CreateGuideWeeklyScheduleCommandValidator : AbstractValidator<CreateGuideWeeklyScheduleCommand>
{
    public CreateGuideWeeklyScheduleCommandValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .Must(day => day >= DayOfWeek.Sunday && day <= DayOfWeek.Saturday)
            .WithMessage("Invalid day of week.");

        RuleFor(x => x.StartTime)
            .Must(time => time >= TimeSpan.Zero && time < TimeSpan.FromHours(24))
            .WithMessage("Start time must be between 00:00 and 23:59.");

        RuleFor(x => x.EndTime)
            .Must(time => time >= TimeSpan.Zero && time < TimeSpan.FromHours(24))
            .WithMessage("End time must be between 00:00 and 23:59.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time.")
            .When(x => x.StartTime >= TimeSpan.Zero && x.EndTime >= TimeSpan.Zero);
    }
}
