using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule;

public class GuideCityConfiguration : IEntityTypeConfiguration<GuideCity>
{
    public void Configure(EntityTypeBuilder<GuideCity> builder)
    {
        builder.ToTable("GuideCities");

        builder.HasKey(x => new { x.GuideId, x.CityId });

        builder.HasOne(x => x.City)
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
