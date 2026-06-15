using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.GuideModule;

internal class PackageMeetingPointConfiguration : IEntityTypeConfiguration<PackageMeetingPoint>
{
    public void Configure(EntityTypeBuilder<PackageMeetingPoint> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("PackageMeetingPoints");

        builder.Property(x => x.MeetingPointName)
            .HasColumnName("Name")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.Time)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.OwnsOne(x => x.MeetingPoint, locationBuilder =>
        {
            locationBuilder.Property(p => p.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(18, 10)
                .IsRequired();

            locationBuilder.Property(p => p.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(18, 10)
                .IsRequired();
        });
    }
}
