using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule;

public class TourGuideConfiguration : IEntityTypeConfiguration<TourGuide>
{
    public void Configure(EntityTypeBuilder<TourGuide> builder)
    {
        builder.ToTable("TourGuides");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.BaseRate)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.PricingUnit)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(x => x.LicenseNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.LicenseExpiryDate)
            .IsRequired(false);

        builder.Property(x => x.YearsOfExperience)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsSuperGuide)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.LastLocationUpdate)
            .IsRequired(false);

        builder.Property(x => x.ProfessionalLicenseUrl)
            .HasColumnName("ProfessionalLicenseUrl")
            .HasConversion(
                fileUrl => fileUrl != null ? fileUrl.Value : null,
                str => str != null ? FileUrl.Create(str).Value : null
            )
            .IsRequired(false);

        builder.Property(x => x.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(x => x.ReviewCount)
            .IsRequired();

        builder.Property(x => x.CompletedToursCount)
            .IsRequired();

        builder.Property(x => x.IsAvailableForBooking)
            .IsRequired();

        builder.OwnsOne(x => x.LastLocation, geo =>
        {
            geo.Property(g => g.Latitude)
                .HasColumnName("Latitude")
                .HasPrecision(18, 6);

            geo.Property(g => g.Longitude)
                .HasColumnName("Longitude")
                .HasPrecision(18, 6);
        });

        builder.Property(t => t.TransportInfo)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(x => x.CityIds)
            .HasField("_cityIds")
            .HasColumnName("CityIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null!) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.TourPackageIds)
            .HasField("_tourPackageIds")
            .HasColumnName("TourPackageIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null!) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());
    }

    private ValueComparer<IReadOnlyCollection<Guid>> CreateGuidListComparer() =>
        new(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
}