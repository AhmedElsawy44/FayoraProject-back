using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{
    public class GuideTourPackageImageConfiguration : IEntityTypeConfiguration<GuideTourPackageImage>
    {
        public void Configure(EntityTypeBuilder<GuideTourPackageImage> builder)
        {
            builder.ToTable("GuideTourPackageImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne<GuideTourPackage>()
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
