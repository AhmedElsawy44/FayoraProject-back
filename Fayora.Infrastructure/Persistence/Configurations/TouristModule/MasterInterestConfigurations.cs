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
            MasterInterest.CreateForSeed(1, "NATURE", "Nature", "assets/icons/nature.png", 1),
            MasterInterest.CreateForSeed(2, "HISTORICAL", "Historical", "assets/icons/historical.png", 2),
            MasterInterest.CreateForSeed(3, "CULTURAL", "Cultural", "assets/icons/cultural.png", 3),
            MasterInterest.CreateForSeed(4, "ADVENTURE", "Adventure", "assets/icons/adventure.png", 4),
            MasterInterest.CreateForSeed(5, "CAMPING", "Camping", "assets/icons/camping.png", 5),
            MasterInterest.CreateForSeed(6, "WILDLIFE", "Wildlife", "assets/icons/wildlife.png", 6)
        );
    }
}