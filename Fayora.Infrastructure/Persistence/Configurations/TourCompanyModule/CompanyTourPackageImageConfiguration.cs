using Fayora.Domain.Entities.TourCompanyModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TourCompanyModule
{

    public class CompanyTourPackageImageConfiguration : IEntityTypeConfiguration<CompanyTourPackageImage>
    {
        public void Configure(EntityTypeBuilder<CompanyTourPackageImage> builder)
        {
            builder.ToTable("CompanyTourPackageImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
