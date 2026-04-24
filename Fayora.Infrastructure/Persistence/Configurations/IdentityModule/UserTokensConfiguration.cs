using Fayora.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.IdentityModule;

public class UserTokensConfiguration : IEntityTypeConfiguration<UserTokens>
{
    public void Configure(EntityTypeBuilder<UserTokens> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.HashedToken)
               .IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.HashedToken)
               .IsRequired()
               .HasMaxLength(512);

        builder.Property(x => x.DeviceId)
               .HasMaxLength(200);

        builder.Property(x => x.IpAddress)
               .HasMaxLength(50);

        builder.Property(x => x.TokenType)
               .HasConversion<int>();
    }
}