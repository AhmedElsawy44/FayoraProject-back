using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.GuideModule;

internal sealed class GuideWeeklyScheduleConfigurations : IEntityTypeConfiguration<GuideWeeklySchedule>
{
    public void Configure(EntityTypeBuilder<GuideWeeklySchedule> builder)
    {
        builder.ToTable("GuideWeeklySchedules");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.GuideId);
        builder.HasIndex(x => x.DayOfWeek);
        builder.HasIndex(x => new { x.GuideId, x.DayOfWeek })
            .IsUnique();

        builder.Property(x => x.DayOfWeek)
            .HasConversion<int>();

        builder.Property(x => x.StartTime)
            .HasColumnType("time");

        builder.Property(x => x.EndTime)
            .HasColumnType("time");

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
