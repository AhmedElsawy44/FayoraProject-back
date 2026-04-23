using Fayora.Domain.Entities.TouristModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class TouristInterestConfiguration : IEntityTypeConfiguration<TouristInterest>
{
    public void Configure(EntityTypeBuilder<TouristInterest> builder)
    {
        builder.ToTable("TouristInterests");

        builder.HasKey(ti => ti.Id);

        builder.Property(ti => ti.TouristId)
            .IsRequired();

        builder.Property(ti => ti.InterestId)
            .IsRequired();

        builder.HasIndex(ti => new { ti.TouristId, ti.InterestId })
            .IsUnique();

        builder.HasOne<MasterInterest>()
            .WithMany()
            .HasForeignKey(ti => ti.InterestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}