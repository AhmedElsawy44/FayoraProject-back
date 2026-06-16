using Fayora.Domain.Entities.GuideModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Configurations.GuideModule
{
    public class PackageNightConfiguration : IEntityTypeConfiguration<PackageNight>
    {
        public void Configure(EntityTypeBuilder<PackageNight> builder)
        {
            builder.ToTable("PackageNights");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NightNumber).IsRequired();
            builder.Property(x => x.NightDate).IsRequired();
            builder.Property(x => x.HousingUnitId).IsRequired(false);
            builder.Property(x => x.PackageAccommodationId).IsRequired(false);
        }
    }
}
