using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations;

public class VerificationCodeConfigurations : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("VerificationCodes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Target)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(v => v.CodeHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(v => v.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(v => v.ExpiresAt)
            .IsRequired();

        builder.Property(v => v.IsUsed)
            .IsRequired();

        builder.Property(v => v.AttemptCount)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Ignore(v => v.IsExpired);
        builder.Ignore(v => v.IsBlocked);

        builder.HasIndex(v => new { v.Target, v.Type });
    }
}
