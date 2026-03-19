using Fayora.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");

            builder.HasKey(x => x.CityId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(3);
        }
    }
}
