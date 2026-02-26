using Fayora.Domain.Entities.Identity; // تأكد من المسار
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

internal class UserArchiveConfigurations : IEntityTypeConfiguration<UserArchive>
{
    public void Configure(EntityTypeBuilder<UserArchive> builder)
    {
        builder.ToTable("UserArchives");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.OriginalUserId)
            .IsRequired();

        builder.Property(a => a.Email)
            .HasMaxLength(255);

        builder.Property(a => a.Phone)
            .HasMaxLength(20);

        builder.Property(a => a.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.DeletionReason)
            .HasMaxLength(1000);

        builder.Property(a => a.DeletedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FullUserDataBackup)
            .IsRequired();
        
        builder.Property(a => a.OriginalCreatedAt).IsRequired();
        builder.Property(a => a.ArchivedAt).IsRequired();

        builder.HasIndex(a => a.OriginalUserId);

        builder.HasIndex(a => a.Email);

        builder.HasIndex(a => a.Phone);
    }
}