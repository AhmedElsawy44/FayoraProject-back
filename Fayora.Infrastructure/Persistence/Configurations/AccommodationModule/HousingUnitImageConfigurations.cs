using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.AccommodationModule;

public class HousingUnitImageConfiguration : IEntityTypeConfiguration<HousingUnitImage>
{
    public void Configure(EntityTypeBuilder<HousingUnitImage> builder)
    {
        builder.ToTable("HousingUnitImages");

        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.UnitId);

        builder.Property(i => i.UnitId)
               .IsRequired();

        builder.Property(i => i.ImageUrl)
               .IsRequired()
               .HasMaxLength(2048);
    }
}