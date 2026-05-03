using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule;

public class TourCompanyConfiguration : IEntityTypeConfiguration<TourCompany>
{
    public void Configure(EntityTypeBuilder<TourCompany> builder)
    {
        builder.ToTable("TourCompanies");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.AverageRating).IsRequired();
        builder.Property(x => x.ReviewCount).IsRequired();
        builder.Property(x => x.CompletedToursCount).IsRequired();
        builder.Property(x => x.IsAvailableForBooking).IsRequired();
        builder.Property(x => x.ResponseRate);
        builder.Property(x => x.CancellationRate).HasPrecision(5, 2);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.TourPackageIds)
            .HasField("_tourPackageIds")
            .HasColumnName("TourPackageIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null!) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsSuperCompany)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.LicenseClass)
            .HasConversion<int>()
            .IsRequired();

        builder.OwnsOne(x => x.LicenseDocumentUrl, url =>
        {
            url.Property(u => u.Value)
               .HasColumnName("LicenseDocumentUrl")
               .HasMaxLength(2048)
               .IsRequired(false);
        });

        builder.Property(x => x.AdminNotes)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();
    }

    private ValueComparer<IReadOnlyCollection<Guid>> CreateGuidListComparer() =>
        new(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
}