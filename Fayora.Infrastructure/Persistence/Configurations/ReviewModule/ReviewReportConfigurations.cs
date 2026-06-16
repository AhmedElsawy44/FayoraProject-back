using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.ReviewModule;

internal sealed class ReviewReportConfigurations : IEntityTypeConfiguration<ReviewReport>
{
    public void Configure(EntityTypeBuilder<ReviewReport> builder)
    {
        builder.ToTable("ReviewReports");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ReviewId, x.ReporterId })
            .IsUnique();

        builder.Property(x => x.Reason)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.AdditionalNotes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.IsResolved)
            .HasDefaultValue(false);

        builder.Property(x => x.ResolvedAt)
            .IsRequired(false);

        builder.HasOne<Review>()
            .WithMany()
            .HasForeignKey(x => x.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
