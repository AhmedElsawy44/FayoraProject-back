using Fayora.Domain.Entities.AccommodationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.AccommodationModule;

public class UnitOwnerConfiguration : IEntityTypeConfiguration<UnitOwner>
{
    public void Configure(EntityTypeBuilder<UnitOwner> builder)
    {
        builder.ToTable("UnitOwners");

        builder.HasKey(o => o.UserId);

        builder.HasIndex(o => o.UserId)
               .IsUnique();

        builder.Property(o => o.UserId)
               .IsRequired();

        builder.Property(o => o.OwnerType)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(o => o.VerificationStatus)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(o => o.NationalIdUrl)
               .IsRequired()
               .HasMaxLength(2048);

        builder.Property(o => o.CommercialName)
               .HasColumnType("nvarchar(200)");

        builder.Property(o => o.TaxRegistrationNumber)
               .HasMaxLength(50);

        builder.HasIndex(o => o.UserId).IsUnique();
        builder.HasIndex(o => o.VerificationStatus);
    }
}