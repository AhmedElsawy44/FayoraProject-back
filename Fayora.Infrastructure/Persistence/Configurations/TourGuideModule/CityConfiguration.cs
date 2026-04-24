using Fayora.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");

        builder.HasKey(x => x.CityId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CountryCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.OwnsOne(x => x.CenterCoordinates, coord =>
        {
            coord.Property(p => p.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            coord.Property(p => p.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(9, 6)
                .IsRequired();
        });
    }
}
