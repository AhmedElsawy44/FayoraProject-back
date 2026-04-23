using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HousingUnitImageConfiguration : IEntityTypeConfiguration<HousingUnitImage>
{
    public void Configure(EntityTypeBuilder<HousingUnitImage> builder)
    {
        builder.ToTable("HousingUnitImages");
        builder.HasKey(i => i.Id);

        builder.OwnsOne(i => i.ImageUrl, url =>
        {
            url.Property(u => u.Value)
               .HasColumnName("ImageUrl")
               .HasMaxLength(2048)
               .IsRequired();
        });
    }
}