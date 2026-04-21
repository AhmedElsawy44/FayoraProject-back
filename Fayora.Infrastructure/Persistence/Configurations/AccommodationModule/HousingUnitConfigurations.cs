using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.AccommodationModule;

public class HousingUnitConfiguration : IEntityTypeConfiguration<HousingUnit>
{
    public void Configure(EntityTypeBuilder<HousingUnit> builder)
    {
        builder.ToTable("HousingUnits");
        builder.HasKey(h => h.Id);

        builder.HasIndex(h => h.LocationId);
        builder.HasIndex(h => h.OwnerId);
        builder.HasIndex(h => h.Status);

        builder.OwnsOne(h => h.Coordinates, coord =>
        {
            coord.Property(c => c.Latitude)
                 .HasColumnName("Latitude")
                 .IsRequired();

            coord.Property(c => c.Longitude)
                 .HasColumnName("Longitude")
                 .IsRequired();
        });


        builder.Property(h => h.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(h => h.Description)
               .HasMaxLength(1000);

        builder.Property(h => h.AddressDetails)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(h => h.MainImageUrl)
               .HasMaxLength(2048);


        builder.Property(h => h.CheckInTime).HasColumnType("time");
        builder.Property(h => h.CheckOutTime).HasColumnType("time");

        builder.Property(h => h.PricePerNight).HasColumnType("decimal(18,2)");
        builder.Property(h => h.CommissionRate).HasColumnType("decimal(18,2)");
        builder.Property(h => h.Rating).HasColumnType("decimal(3,2)");

        builder.Property(h => h.Type)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(h => h.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Navigation(h => h.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(h => h.Amenities).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(h => h.Images)
               .WithOne()
               .HasForeignKey(i => i.UnitId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Amenities)
               .WithOne()
               .HasForeignKey(ua => ua.UnitId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}