using System;
using System.Linq;

namespace Fayora.Application.Common.Helpers;

public static class CronExpressionHelper
{
    public static string? GenerateCron(string scheduleType, TimeSpan? preferredTime, string? daysOfWeek, int? dayOfMonth, string? customCron)
    {
        if (string.Equals(scheduleType, "Custom", StringComparison.OrdinalIgnoreCase))
        {
            return customCron;
        }

        var time = preferredTime ?? TimeSpan.Zero;
        int minute = time.Minutes;
        int hour = time.Hours;

        if (string.Equals(scheduleType, "Daily", StringComparison.OrdinalIgnoreCase))
        {
            return $"{minute} {hour} * * *";
        }

        if (string.Equals(scheduleType, "Weekly", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(daysOfWeek))
                throw new ArgumentException("Days of week must be specified for weekly schedule.");

            var mappedDays = daysOfWeek.Split(',')
                .Select(d => d.Trim())
                .Select(MapDayOfWeek)
                .ToList();

            var daysString = string.Join(",", mappedDays);
            return $"{minute} {hour} * * {daysString}";
        }

        if (string.Equals(scheduleType, "Monthly", StringComparison.OrdinalIgnoreCase))
        {
            int day = dayOfMonth ?? 1;
            if (day < 1 || day > 31)
                throw new ArgumentOutOfRangeException(nameof(dayOfMonth), "Day of month must be between 1 and 31.");

            return $"{minute} {hour} {day} * *";
        }

        throw new ArgumentException($"Invalid schedule type: {scheduleType}");
    }

    private static string MapDayOfWeek(string day)
    {
        if (int.TryParse(day, out int dayNum))
        {
            if (dayNum >= 0 && dayNum <= 6)
                return dayNum.ToString();
        }

        return day.ToLower() switch
        {
            "sunday" or "sun" => "0",
            "monday" or "mon" => "1",
            "tuesday" or "tue" => "2",
            "wednesday" or "wed" => "3",
            "thursday" or "thu" => "4",
            "friday" or "fri" => "5",
            "saturday" or "sat" => "6",
            _ => throw new ArgumentException($"Invalid day of week: {day}")
        };
    }
}
