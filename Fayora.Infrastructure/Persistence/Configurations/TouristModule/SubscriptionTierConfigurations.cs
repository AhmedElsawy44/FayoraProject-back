using Fayora.Domain.Entitties.Tourist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class SubscriptionTierConfiguration : IEntityTypeConfiguration<SubscriptionTier>
{
    public void Configure(EntityTypeBuilder<SubscriptionTier> builder)
    {
        builder.ToTable("SubscriptionTiers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PlanCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(s => s.PlanCode)
            .IsUnique();

        builder.Property(s => s.PlanName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.AIChatLimit)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();
    }
}