using Fayora.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.IdentityModule;

public class VerificationDocumentConfigurations : IEntityTypeConfiguration<VerificationDocument>
{
    public void Configure(EntityTypeBuilder<VerificationDocument> builder)
    {
        builder.ToTable("VerificationDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.DocumentUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(d => d.DocumentStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(d => d.RejectionReason)
            .HasMaxLength(500);

        builder.Property(d => d.RequestId)
            .IsRequired();

        builder.HasIndex(d => d.RequestId);
    }
}
