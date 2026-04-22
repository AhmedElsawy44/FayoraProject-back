using Fayora.Domain.Entities.TourCompanyModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.TourCompanyModule
{
    public class CompanyTourPackageConfiguration : IEntityTypeConfiguration<CompanyTourPackage>
    {
        public void Configure(EntityTypeBuilder<CompanyTourPackage> builder)
        {
            builder.ToTable("CompanyTourPackages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.TourTypes)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.AdultPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.ChildPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.MainImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.MainVideoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.CancellationPolicy)
                .HasMaxLength(1000);

            builder.OwnsOne(x => x.DepartureLocation, geo =>
            {
                geo.Property(g => g.Latitude)
                    .HasColumnName("DepartureLatitude")
                    .HasPrecision(18, 6);
                geo.Property(g => g.Longitude)
                    .HasColumnName("DepartureLongitude")
                    .HasPrecision(18, 6);
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

            // Relationships
            builder.HasMany(x => x.Images)
                .WithOne()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Activities)
                .WithOne()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
