using Fayora.Domain.Entities.TouristModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class TouristProfileConfiguration : IEntityTypeConfiguration<TouristProfile>
{
    public void Configure(EntityTypeBuilder<TouristProfile> builder)
    {
        builder.ToTable("TouristProfiles");

        builder.HasKey(tp => tp.Id);

        builder.HasIndex(tp => tp.UserId)
            .IsUnique();

        builder.Property(tp => tp.TravelStyle)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(tp => tp.BudgetTier)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.OwnsOne(tp => tp.LastLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("LastLocationLatitude")
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("LastLocationLongitude")
                .IsRequired();
        });

        builder.Property(tp => tp.CreatedAt)
            .IsRequired();

        builder.Property(tp => tp.OnboardingComplete)
            .IsRequired();
    }
}