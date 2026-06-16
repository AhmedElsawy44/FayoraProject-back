using Fayora.Domain.Entities.NotificationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.NotificationModule;

public class InAppNotificationConfiguration : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("InAppNotifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.IsRead)
            .IsRequired();

        builder.Property(n => n.Type)
            .HasMaxLength(50);

        builder.Property(n => n.EntityId)
            .HasMaxLength(100);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
    }
}
