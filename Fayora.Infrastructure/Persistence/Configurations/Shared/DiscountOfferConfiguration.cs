using Fayora.Domain.Entities.SharedModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations.Shared
{
    public class DiscountOfferConfiguration : IEntityTypeConfiguration<DiscountOffer>
    {
        public void Configure(EntityTypeBuilder<DiscountOffer> builder)
        {
            builder.ToTable("DiscountOffers");
            builder.HasKey(o => o.Id);

            builder.HasIndex(o => o.TargetId);
            builder.HasIndex(o => o.OwnerId);
            builder.HasIndex(o => new { o.TargetId, o.TargetType, o.Status });

            builder.Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Description)
                .HasMaxLength(500);

            builder.Property(o => o.DiscountValue)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.DiscountType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.TargetType)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();


            builder.Property(o => o.StartDate).IsRequired();
            builder.Property(o => o.EndDate).IsRequired();
            builder.Property(o => o.OwnerId).IsRequired();
            builder.Property(o => o.TargetId).IsRequired();

            builder.Property(o => o.UsageLimit)
                .IsRequired(false);

            builder.Property(o => o.UsageCount)
                .HasDefaultValue(0)
                .IsRequired();
        }
    }
}
