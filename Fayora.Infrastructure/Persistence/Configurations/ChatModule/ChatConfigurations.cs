using Fayora.Domain.Entities.ChatModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.ChatModule;

public class ChatConfigurations : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstUserId)
            .IsRequired();

        builder.Property(c => c.SecondUserId)
            .IsRequired();

        builder.Property(c => c.ScopeId)
            .IsRequired();

        builder.Property(c => c.ScopeType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(Chat.Messages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey("ChatId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.FirstUserId, c.SecondUserId, c.ScopeType, c.ScopeId })
            .HasDatabaseName("IX_Chats_Participants_Scope");
    }
}