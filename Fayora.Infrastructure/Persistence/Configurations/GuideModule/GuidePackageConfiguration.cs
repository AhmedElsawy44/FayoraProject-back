using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

public class GuidePackageConfiguration : IEntityTypeConfiguration<GuidePackage>
{
    public void Configure(EntityTypeBuilder<GuidePackage> builder)
    {
        builder.ToTable("GuideTourPackages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired()
            .HasColumnType("nvarchar(255)");
        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(2000)");
        builder.Property(x => x.AdultPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.ChildPrice).HasPrecision(18, 2);
        builder.Property(x => x.ProviderType).HasConversion<int>();
        builder.Property(x => x.CancellationPolicy).HasConversion<int>();
        builder.Property(x => x.TransportType).HasConversion<int>();
        builder.Property(x => x.PackageStatus).HasConversion<int>();
        builder.Property(x => x.GuestRequirements)
            .HasColumnType("nvarchar(1000)");
        builder.Property(x => x.ArrivalNote)
            .HasColumnType("nvarchar(1000)");

        builder.OwnsOne(x => x.MainImageUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainImageUrl").HasMaxLength(2048);
        });

        builder.OwnsOne(x => x.MainVideoUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainVideoUrl").HasMaxLength(2048);
        });

        builder.OwnsOne(x => x.MeetingPoint, geo =>
        {
            geo.Property(g => g.Latitude).HasColumnName("MeetingPointLatitude").HasPrecision(18, 6);
            geo.Property(g => g.Longitude).HasColumnName("MeetingPointLongitude").HasPrecision(18, 6);
        });

        builder.Property<List<int>>("_includedItemIds")
            .HasColumnName("IncludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
            .Metadata.SetValueComparer(CreateIntListComparer());

        builder.Property<List<int>>("_excludedItemIds")
            .HasColumnName("ExcludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
            .Metadata.SetValueComparer(CreateIntListComparer());

        builder.Property<List<Guid>>("_imageIds")
            .HasColumnName("ImageIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.TourTypes).HasConversion<int>();

        builder.Property<List<Guid>>("_activityIds")
            .HasColumnName("ActivityIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.TourTypes).HasConversion<int>();

        builder.HasMany(p => p.Occurrences)
               .WithOne()
               .HasForeignKey(o => o.PackageId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(GuidePackage.Occurrences))
       ?.SetPropertyAccessMode(PropertyAccessMode.Field);



        builder.Property<List<int>>("_locationIds")
              .HasColumnName("LocationIds")
              .HasConversion(
              v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
              v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
             .Metadata.SetValueComparer(CreateIntListComparer());

    }

    private ValueComparer<List<int>> CreateIntListComparer()
    {
        ValueComparer<List<int>> valueComparer = new((c1, c2) => c1!.SequenceEqual(c2!), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToList());
        return valueComparer;
    }

    private ValueComparer<List<Guid>> CreateGuidListComparer()
    {
        ValueComparer<List<Guid>> valueComparer = new((c1, c2) => c1!.SequenceEqual(c2!), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToList());
        return valueComparer;
    }
}