using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{
    public class GuideTourPackageConfiguration : IEntityTypeConfiguration<GuideTourPackage>
    {
        public void Configure(EntityTypeBuilder<GuideTourPackage> builder)
        {
            builder.ToTable("GuideTourPackages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.AdultPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.DurationHours)
                .IsRequired();

            builder.Property(x => x.MaxCapacity)
                .IsRequired();

            builder.OwnsOne(x => x.MeetingPoint, geo =>
            {
                geo.Property(g => g.Latitude).HasColumnName("MeetingPointLatitude");
                geo.Property(g => g.Longitude).HasColumnName("MeetingPointLongitude");
            });


            var listComparer = new ValueComparer<IReadOnlyCollection<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Property(x => x.IncludedItems)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .Metadata.SetValueComparer(listComparer);

            builder.Property(x => x.ExcludedItems)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!)
                .Metadata.SetValueComparer(listComparer);


        }
    }
}
