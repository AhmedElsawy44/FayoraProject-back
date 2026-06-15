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

        builder.Property(x => x.HasGroupDiscount).HasDefaultValue(false);
        builder.Property(x => x.GroupDiscountMinPeople).IsRequired(false);
        builder.Property(x => x.GroupDiscountPercent).HasPrecision(18, 2).IsRequired(false);

        builder.OwnsOne(x => x.MainImageUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainImageUrl").HasMaxLength(2048);
        });

        builder.OwnsOne(x => x.MainVideoUrl, nav =>
        {
            nav.Property(f => f.Value).HasColumnName("MainVideoUrl").HasMaxLength(2048);
        });



        builder.Property<List<int>>("_includedItemIds")
            .HasColumnName("IncludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v) ? new List<int>() : (JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>()))
            .Metadata.SetValueComparer(CreateIntListComparer());

        builder.Property<List<int>>("_excludedItemIds")
            .HasColumnName("ExcludedItemIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v) ? new List<int>() : (JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>()))
            .Metadata.SetValueComparer(CreateIntListComparer());

        builder.Property<List<Guid>>("_imageIds")
            .HasColumnName("ImageIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v) ? new List<Guid>() : (JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()))
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property(x => x.TourTypes).HasConversion<int>();

        builder.Property<List<Guid>>("_activityIds")
            .HasColumnName("ActivityIds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v) ? new List<Guid>() : (JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()))
            .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property<List<Guid>>("_nightIds")
           .HasColumnName("NightIds")
           .HasConversion(
               v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
               v => string.IsNullOrWhiteSpace(v) ? new List<Guid>() : (JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()))
           .Metadata.SetValueComparer(CreateGuidListComparer());

        builder.Property<List<OptionalActivity>>("_optionalActivities")
            .HasColumnName("OptionalActivities")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<OptionalActivity>>(v, (JsonSerializerOptions?)null) ?? new List<OptionalActivity>())
            .Metadata.SetValueComparer(CreateOptionalActivitiesComparer());



        builder.Property(x => x.TourTypes).HasConversion<int>();

        builder.HasMany(p => p.Occurrences)
               .WithOne()
               .HasForeignKey(o => o.PackageId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(GuidePackage.Occurrences))
       ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.MeetingPoints)
               .WithOne()
               .HasForeignKey(mp => mp.PackageId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(GuidePackage.MeetingPoints))
       ?.SetPropertyAccessMode(PropertyAccessMode.Field);



        builder.Property<List<int>>("_locationIds")
              .HasColumnName("LocationIds")
              .HasConversion(
              v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
              v => string.IsNullOrWhiteSpace(v) ? new List<int>() : (JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>()))
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

    private ValueComparer<List<OptionalActivity>> CreateOptionalActivitiesComparer()
    {
        ValueComparer<List<OptionalActivity>> valueComparer = new((c1, c2) => c1!.SequenceEqual(c2!), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToList());
        return valueComparer;
    }
}