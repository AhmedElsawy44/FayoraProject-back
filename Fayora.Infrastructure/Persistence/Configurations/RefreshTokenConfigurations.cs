using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.DeviceId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.IpAddress)
            .HasMaxLength(45);

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.HasIndex(r => r.Token)
            .IsUnique();

        builder.HasIndex(r => r.UserId);

        builder.HasIndex(r => r.DeviceId);

        builder.HasIndex(r => new { r.Token, r.DeviceId });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
