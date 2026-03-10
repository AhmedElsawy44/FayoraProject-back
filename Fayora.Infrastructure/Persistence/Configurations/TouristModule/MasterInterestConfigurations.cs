using Fayora.Domain.Entitties.Tourist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class MasterInterestConfiguration : IEntityTypeConfiguration<MasterInterest>
{
    public void Configure(EntityTypeBuilder<MasterInterest> builder)
    {
        builder.ToTable("MasterInterests");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(m => m.Code)
            .IsUnique();

        builder.Property(m => m.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.IconUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(m => m.CreateAt)
            .IsRequired();

        builder.HasData(
            MasterInterest.CreateForSeed(1, "NATURE", "Nature", "wwwroot/images/interests/nature.png", 1),
            MasterInterest.CreateForSeed(2, "HISTORICAL", "Historical", "wwwroot/images/interests/historical.png", 2),
            MasterInterest.CreateForSeed(3, "CULTURAL", "Cultural", "wwwroot/images/interests/cultural.png", 3),
            MasterInterest.CreateForSeed(4, "ADVENTURE", "Adventure", "wwwroot/images/interests/adventure.png", 4),
            MasterInterest.CreateForSeed(5, "CAMPING", "Camping", "wwwroot/images/interests/camping.png", 5),
            MasterInterest.CreateForSeed(6, "WILDLIFE", "Wildlife", "wwwroot/images/interests/wildlife.png", 6)
        );
    }
}