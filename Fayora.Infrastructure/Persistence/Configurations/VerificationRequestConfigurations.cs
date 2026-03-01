using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

public class VerificationRequestConfigurations : IEntityTypeConfiguration<VerificationRequest>
{
    public void Configure(EntityTypeBuilder<VerificationRequest> builder)
    {
        builder.ToTable("VerificationRequests");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.RequestType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.RequestStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.AdminComment)
            .HasMaxLength(1000);

        var navigation = builder.Metadata.FindNavigation(nameof(VerificationRequest.VerificationDocuments));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(v => v.VerificationDocuments)
            .WithOne()
            .HasForeignKey("VerificationRequestId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.UserId);

        builder.HasIndex(v => v.RequestStatus);
    }
}