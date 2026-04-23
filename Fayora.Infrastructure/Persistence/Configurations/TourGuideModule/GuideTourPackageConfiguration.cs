using Fayora.Domain.Entities.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

public class GuideTourPackageConfiguration : IEntityTypeConfiguration<GuidePackage>
{
    public void Configure(EntityTypeBuilder<GuidePackage> builder)
    {
        builder.ToTable("GuideTourPackages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.AdultPrice).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.ChildPrice).HasPrecision(18, 2);

        // Mapping الـ Value Objects (MainImageUrl & MainVideoUrl)
        builder.OwnsOne(x => x.MainImageUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainImageUrl").HasMaxLength(2048);
        });

        builder.OwnsOne(x => x.MainVideoUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainVideoUrl").HasMaxLength(2048);
        });

        // Meeting Point
        builder.OwnsOne(x => x.MeetingPoint, geo =>
        {
            geo.Property(g => g.Latitude).HasColumnName("MeetingPointLatitude").HasPrecision(18, 6);
            geo.Property(g => g.Longitude).HasColumnName("MeetingPointLongitude").HasPrecision(18, 6);
        });

        // التعامل مع الـ Lists (Ids Only) كـ JSON Columns
        // 1. IncludedItemIds
        builder.Property<List<int>>("_includedItemIds")
            .HasColumnName("IncludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
            .Metadata.SetValueComparer(CreateIntListComparer());

        // 2. ExcludedItemIds
        builder.Property<List<int>>("_excludedItemIds")
            .HasColumnName("ExcludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
            .Metadata.SetValueComparer(CreateIntListComparer());

        // 3. ImageIds (Guids)
        builder.Property<List<Guid>>("_imageIds")
            .HasColumnName("ImageIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.TourTypes).HasConversion<int>();
    }

    // Helper methods for Comparers
    private ValueComparer<List<int>> CreateIntListComparer() =>
        new ValueComparer<List<int>>((c1, c2) => c1!.SequenceEqual(c2!), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToList());

    private ValueComparer<List<Guid>> CreateGuidListComparer() =>
        new ValueComparer<List<Guid>>((c1, c2) => c1!.SequenceEqual(c2!), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToList());
}