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

        builder.Property(a => a.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(a => a.Icon, iconBuilder =>
        {
            iconBuilder.Property(f => f.Value)
                .HasColumnName("IconUrl")
                .HasMaxLength(2048)
                .IsRequired(false);
        });

        builder.Property(a => a.Category)
            .HasConversion<int>()
            .IsRequired();
    }
}