using Fayora.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.BookingModule;

internal sealed class ProviderPayoutConfigurations : IEntityTypeConfiguration<ProviderPayout>
{
    public void Configure(EntityTypeBuilder<ProviderPayout> builder)
    {
        builder.ToTable("ProviderPayouts");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ProviderId);
        builder.HasIndex(x => x.PayoutDate);

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.PayoutDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();
    }
}
