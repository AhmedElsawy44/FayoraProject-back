using Fayora.Domain.Entities.TourGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourGuideModule
{

    public class GuideRequestConfiguration : IEntityTypeConfiguration<GuideRequest>
    {
        public void Configure(EntityTypeBuilder<GuideRequest> builder)
        {
            builder.ToTable("GuideRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TripPlanId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.BudgetAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.NumberOfPeople)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.OwnsOne(x => x.MeetingPoint, geo =>
            {
                geo.Property(g => g.Latitude)
                    .HasColumnName("MeetingPointLatitude")
                    .HasPrecision(18, 6);
                geo.Property(g => g.Longitude)
                    .HasColumnName("MeetingPointLongitude")
                    .HasPrecision(18, 6);
            });

            builder.HasMany(x => x.GuideOffers)
                .WithOne()
                .HasForeignKey(x => x.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
