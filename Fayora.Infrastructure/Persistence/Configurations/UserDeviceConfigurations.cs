using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

partial class UserDeviceConfigurations : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable("UserDevices");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DeviceId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(d => d.FCMToken)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(d => d.DeviceType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.DeviceModel)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.DeviceLanguage)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastUsedAt)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasIndex(d => d.UserId);

        builder.HasIndex(d => d.FCMToken);

        builder.HasIndex(d => new { d.UserId, d.DeviceId })
            .IsUnique();
    }
}
