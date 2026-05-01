using Fayora.Domain.Entities.ChatModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.ChatModule;

// خليناه public عشان الـ Assembly Scanning يقدر يلاقيه لو بتستخدم ApplyConfigurationsFromAssembly
public class MessageConfigurations : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .UseIdentityColumn();

        builder.Property(m => m.ChatId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.ReadAt)
            .IsRequired(false);

        builder.HasIndex(m => m.ChatId)
            .HasDatabaseName("IX_Messages_ChatId");

        builder.HasIndex(m => new { m.ChatId, m.CreatedAt });
    }
}