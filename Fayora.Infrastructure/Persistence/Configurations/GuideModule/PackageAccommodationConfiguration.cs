using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations.GuideModule
{
    public class PackageAccommodationConfiguration : IEntityTypeConfiguration<PackageAccommodation>
    {
        public void Configure(EntityTypeBuilder<PackageAccommodation> builder)
        {
            builder.ToTable("PackageAccommodations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .HasConversion<int>();

            builder.Property(x => x.Amenities)
                .HasConversion<int>();

            builder.Property(x => x.Meals)
                .HasConversion<int>();

            builder.OwnsOne(x => x.MainImageUrl, nav =>
            {
                nav.Property(f => f.Value)
                   .HasColumnName("MainImageUrl")
                   .HasMaxLength(2048);
            });

            builder.OwnsOne(x => x.Location, geo =>
            {
                geo.Property(g => g.Latitude)
                   .HasColumnName("Latitude")
                   .HasPrecision(18, 6);
                geo.Property(g => g.Longitude)
                   .HasColumnName("Longitude")
                   .HasPrecision(18, 6);
            });


            builder.OwnsMany(x => x.GalleryImages, nav =>
            {
                nav.WithOwner().HasForeignKey("PackageAccommodationId");
                nav.Property(f => f.Value)
                   .HasColumnName("Url")
                   .HasMaxLength(2048);
                nav.ToTable("PackageAccommodationGalleryImages");
            });
        }
    }
}
