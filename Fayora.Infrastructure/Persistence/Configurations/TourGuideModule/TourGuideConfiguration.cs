using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{
    public class TourGuideConfiguration : IEntityTypeConfiguration<TourGuide>
    {
        public void Configure(EntityTypeBuilder<TourGuide> builder)
        {
            builder.ToTable("TourGuides");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.BaseRate)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.PricingUnit)
                .IsRequired();

            builder.Property(x => x.LicenseNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LicenseExpiryDate)
                .IsRequired();

            builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.TaxRegistrationNumber)
                .HasMaxLength(100);

            builder.Property(x => x.AverageRating)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CancellationRate)
                .HasPrecision(5, 2);

            builder.OwnsOne(x => x.LastLocation, geo =>
            {
                geo.Property(g => g.Latitude)
                    .HasColumnName("Latitude")
                    .HasPrecision(18, 6);
                geo.Property(g => g.Longitude)
                    .HasColumnName("Longitude")
                    .HasPrecision(18, 6);
            });

            builder.Property(x => x.CancellationRate)
                .HasPrecision(5, 2);

            builder.OwnsOne(x => x.TransportInfo, transport =>
            {
                transport.Property(t => t.HasOwnVehicle).HasColumnName("HasOwnVehicle");
                transport.Property(t => t.VehicleDetails).HasColumnName("VehicleDetails");
                transport.Property(t => t.TransportType).HasColumnName("TransportType");
            });

            // Relationships
            builder.HasMany(x => x.GuideCities)
                .WithOne(x => x.TourGuide)
                .HasForeignKey(x => x.GuideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TourPackages)
                .WithOne()
                .HasForeignKey(x => x.GuideId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

