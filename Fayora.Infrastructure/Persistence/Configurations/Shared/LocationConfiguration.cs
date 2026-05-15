using Fayora.Domain.Entities.SharedModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.Shared
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.OwnsOne(x => x.Coordinates, nav =>
            {
                nav.Property(c => c.Latitude)
                   .HasColumnName("Latitude")
                   .IsRequired();

                nav.Property(c => c.Longitude)
                   .HasColumnName("Longitude")
                   .IsRequired();
            });

            builder.OwnsOne(x => x.MainImageUrl, nav =>
            {
                nav.Property(f => f.Value)
                   .HasColumnName("MainImageUrl")
                   .IsRequired()
                   .HasMaxLength(500);
            });

            builder.Property(x => x.Rating)
                .HasPrecision(3, 2);

            builder.Property<List<Guid>>("_imageIds")
                 .HasColumnName("ImageIds")
                 .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
                 .Metadata.SetValueComparer(new ValueComparer<List<Guid>>(
                 (c1, c2) => c1!.SequenceEqual(c2!),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                 c => c.ToList()));
        }
    }
}
