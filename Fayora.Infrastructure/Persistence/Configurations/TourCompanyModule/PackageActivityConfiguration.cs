using Fayora.Domain.Entities.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourCompanyModule
{
    public class PackageActivityConfiguration : IEntityTypeConfiguration<PackageActivity>
    {
        public void Configure(EntityTypeBuilder<PackageActivity> builder)
        {
            builder.ToTable("PackageActivities");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PackageId)
                .IsRequired();

            builder.Property(x => x.PlaceId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.DurationHours)
                .IsRequired();
        }
    }
}
