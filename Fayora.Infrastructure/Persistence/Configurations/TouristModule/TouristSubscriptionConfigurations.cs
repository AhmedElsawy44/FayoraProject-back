using Fayora.Domain.Entitties.Tourist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class TouristSubscriptionConfiguration : IEntityTypeConfiguration<TouristSubscription>
{
    public void Configure(EntityTypeBuilder<TouristSubscription> builder)
    {
        builder.ToTable("TouristSubscriptions");

        builder.HasKey(ts => ts.Id);

        builder.HasIndex(ts => new { ts.UserId, ts.Status, ts.EndDate })
            .HasDatabaseName("IX_TouristSubscriptions_ActiveUserPlan");

        builder.Property(ts => ts.UserId)
            .IsRequired();

        builder.Property(ts => ts.PlanId)
            .IsRequired();

        builder.Property(ts => ts.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ts => ts.AIChatLimit)
            .IsRequired();

        builder.Property(ts => ts.AiMessagesUsed)
            .IsRequired();

        builder.Property(ts => ts.StartDate).IsRequired();
        builder.Property(ts => ts.EndDate).IsRequired();
        builder.Property(ts => ts.CreateAt).IsRequired();

        builder.Property(ts => ts.AutoRenew).IsRequired();
    }
}