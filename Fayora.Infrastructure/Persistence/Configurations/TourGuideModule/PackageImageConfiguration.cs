using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PackageImageConfiguration : IEntityTypeConfiguration<PackageImage>
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