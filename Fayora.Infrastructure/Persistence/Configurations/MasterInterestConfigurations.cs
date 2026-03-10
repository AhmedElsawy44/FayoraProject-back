using Fayora.Domain.Entitties.Tourist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

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
    }
}