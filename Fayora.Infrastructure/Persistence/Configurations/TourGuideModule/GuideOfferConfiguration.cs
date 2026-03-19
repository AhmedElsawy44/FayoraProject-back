using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{
    public class GuideOfferConfiguration : IEntityTypeConfiguration<GuideOffer>
    {
        public void Configure(EntityTypeBuilder<GuideOffer> builder)
        {
            builder.ToTable("GuideOffers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestId)
                .IsRequired();

            builder.Property(x => x.GuideId)
                .IsRequired();

            builder.Property(x => x.ProposedPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.Message)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired();
        }
    }
}
