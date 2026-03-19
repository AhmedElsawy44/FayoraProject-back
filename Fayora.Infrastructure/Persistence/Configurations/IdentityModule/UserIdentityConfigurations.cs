using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.IdentityModule;

public class UserIdentityConfigurations : IEntityTypeConfiguration<UserIdentity>
{
    public void Configure(EntityTypeBuilder<UserIdentity> builder)
    {
        builder.ToTable("UserIdentities");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserId)
            .IsRequired();

        builder.Property(u => u.Provider)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion<string>();

        builder.Property(u => u.ProviderKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Email)
            .IsRequired(false)
            .HasConversion(
                u => u == null ? null : u.Value,
                v => v == null ? null : Email.Create(v).Value)
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email);

        builder.Property(u => u.LinkedAt)
            .IsRequired();

        builder.HasIndex(u => u.Email);

        builder.HasIndex(u => new { u.Provider, u.ProviderKey })
            .IsUnique();

        builder.HasIndex(u => new { u.UserId, u.Provider })
            .IsUnique();
    }
}
