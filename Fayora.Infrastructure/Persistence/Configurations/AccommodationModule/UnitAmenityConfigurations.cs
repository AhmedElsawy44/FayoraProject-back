using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.AccommodationModule;

public class UnitAmenityConfiguration : IEntityTypeConfiguration<UnitAmenity>
{
    public void Configure(EntityTypeBuilder<UnitAmenity> builder)
    {
        builder.ToTable("UnitAmenities");

        builder.HasKey(ua => ua.Id);

        builder.HasIndex(ua => new { ua.UnitId, ua.AmenityId })
               .IsUnique();

        builder.HasIndex(ua => ua.AmenityId);

        builder.HasOne<HousingUnit>()
               .WithMany(h => h.Amenities)
               .HasForeignKey(ua => ua.UnitId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<MasterAmenity>()
               .WithMany()
               .HasForeignKey(ua => ua.AmenityId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}