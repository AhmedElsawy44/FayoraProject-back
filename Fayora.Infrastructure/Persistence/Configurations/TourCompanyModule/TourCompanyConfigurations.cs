using Fayora.Domain.Entities.TourGuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations.TourCompanyModule
{
    public class TourCompanyConfiguration : IEntityTypeConfiguration<TourCompany>
    {
        public void Configure(EntityTypeBuilder<TourCompany> builder)
        {
            builder.ToTable("TourCompanies");

            builder.HasKey(x => x.UserId);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.AverageRating)
                .IsRequired();

            builder.Property(x => x.ReviewCount)
                .IsRequired();

            builder.Property(x => x.CompletedToursCount)
                .IsRequired();

            builder.Property(x => x.IsAvailableForBooking)
                .IsRequired();

            builder.Property(x => x.ResponseRate)
                .HasPrecision(5, 2);

            builder.Property(x => x.CancellationRate)
                .HasPrecision(5, 2);

            builder.Property(x => x.Status)
                .IsRequired();

            // Relationships
            builder.HasMany(x => x.Packages)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
