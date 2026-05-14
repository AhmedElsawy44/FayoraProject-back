using Fayora.Domain.Entities.SharedModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.Shared
{
    public class LocationImageConfiguration : IEntityTypeConfiguration<LocationImage>
    {
        public void Configure(EntityTypeBuilder<LocationImage> builder)
        {
            builder.ToTable("LocationImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LocationId)
                .IsRequired();

            builder.OwnsOne(x => x.ImageUrl, nav =>
            {
                nav.Property(f => f.Value)
                   .HasColumnName("ImageUrl")
                   .IsRequired()
                   .HasMaxLength(500);
            });
        }
    }
}
