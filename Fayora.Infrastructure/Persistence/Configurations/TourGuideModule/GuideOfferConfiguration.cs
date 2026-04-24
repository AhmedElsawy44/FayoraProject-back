using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
