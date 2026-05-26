using Fayora.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.BookingModule;

internal sealed class BookingConfigurations : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ServiceProviderId);
        builder.HasIndex(x => x.ServiceId);
        builder.HasIndex(x => x.ServiceType);
        builder.HasIndex(x => x.BookingStatus);
        builder.HasIndex(x => x.PaymentStatus);
        builder.HasIndex(x => x.StartDate);
        builder.HasIndex(x => x.EndDate);
        builder.HasIndex(x => x.AppliedOfferId);  // ✅ مفيد للـ analytics

        builder.Property(x => x.ServiceType)
            .HasConversion<int>();

        builder.Property(x => x.AppliedCancelPolicy)
            .HasConversion<int>();

        builder.Property(x => x.BookingStatus)
            .HasConversion<int>();

        builder.Property(x => x.PaymentStatus)
            .HasConversion<int>();

        builder.Property(x => x.BasePrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ServiceFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PayoutAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)");


        builder.Property(x => x.DiscountAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(x => x.AppliedOfferId)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();
    }
}