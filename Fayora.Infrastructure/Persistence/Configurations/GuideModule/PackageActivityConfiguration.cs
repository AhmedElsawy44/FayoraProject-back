using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.GuideModule;

internal class PackageActivityConfiguration : IEntityTypeConfiguration<PackageActivity>
{
    public void Configure(EntityTypeBuilder<PackageActivity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.ToTable("PackageActivities");

        builder.Property(a => a.Description)
               .HasMaxLength(500)
               .IsRequired(false);

        builder.Property(a => a.ActivityTime)
               .IsRequired();

        builder.OwnsOne(a => a.Place, placeBuilder =>
        {
            placeBuilder.Property(p => p.Latitude)
                        .HasColumnName("Latitude")
                        .HasPrecision(18, 10)
                        .IsRequired();

            placeBuilder.Property(p => p.Longitude)
                        .HasColumnName("Longitude")
                        .HasPrecision(18, 10)
                        .IsRequired();
        });
    }
}