using Fayora.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.BookingModule;

internal sealed class CalendarBlockConfigurations : IEntityTypeConfiguration<CalendarBlock>
{
    public void Configure(EntityTypeBuilder<CalendarBlock> builder)
    {
        builder.ToTable("CalendarBlocks");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ServiceId);
        builder.HasIndex(x => x.ServiceType);
        builder.HasIndex(x => x.StartDate);
        builder.HasIndex(x => x.EndDate);
        builder.HasIndex(x => x.BlockReason);
        builder.HasIndex(x => new { x.ServiceId, x.StartDate, x.EndDate });

        builder.Property(x => x.ServiceType)
            .HasConversion<int>();

        builder.Property(x => x.BlockReason)
            .HasConversion<int>()
            .HasDefaultValue(1);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
