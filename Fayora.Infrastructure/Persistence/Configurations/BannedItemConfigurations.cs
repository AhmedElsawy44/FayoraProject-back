using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

public class BannedItemConfigurations : IEntityTypeConfiguration<BannedItem>
{
    public void Configure(EntityTypeBuilder<BannedItem> builder)
    {
        builder.ToTable("BannedItems");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BanType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.BanValue)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.Reason)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.HasIndex(b => new { b.BanType, b.BanValue });

        builder.HasIndex(b => b.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
