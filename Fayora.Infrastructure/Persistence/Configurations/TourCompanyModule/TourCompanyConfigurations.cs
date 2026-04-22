using Fayora.Domain.Entities.TourCompanyModule;
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

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.CommercialRegisterNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.TaxRegistrationNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.LogoUrl)
                .HasMaxLength(500);

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
