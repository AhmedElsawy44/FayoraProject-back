using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.AccommodationModule;

public class MasterAmenityConfiguration : IEntityTypeConfiguration<MasterAmenity>
{
    public void Configure(EntityTypeBuilder<MasterAmenity> builder)
    {
        builder.ToTable("MasterAmenities");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).UseIdentityColumn();

        builder.Property(a => a.Name).IsRequired().HasColumnType("nvarchar(100)");
        builder.Property(a => a.IconUrl).IsRequired().HasMaxLength(2048);
        builder.Property(a => a.Category).IsRequired().HasMaxLength(100);
    }
}