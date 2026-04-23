using Fayora.Domain.Entities.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GuideTourPackageImageConfiguration : IEntityTypeConfiguration<PackageImage>
{
    public void Configure(EntityTypeBuilder<PackageImage> builder)
    {
        builder.ToTable("PackageImages");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.ImageUrl, nav =>
        {
            nav.Property(f => f.Value)
               .HasColumnName("ImageUrl")
               .IsRequired()
               .HasMaxLength(500);
        });
    }
}