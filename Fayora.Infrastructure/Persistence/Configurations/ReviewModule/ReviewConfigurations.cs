using Fayora.Domain.Entities.ReviewModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.ReviewModule;

internal sealed class ReviewConfigurations : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.BookingId)
            .IsUnique();

        builder.HasIndex(x => x.ReviewerId);
        builder.HasIndex(x => new { x.TargetId, x.TargetType });

        builder.Property(x => x.TargetType)
            .HasConversion<int>();

        builder.Property(x => x.Rating)
            .HasColumnType("decimal(3,1)")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(x => x.Reviewer)
            .WithMany()
            .HasForeignKey(x => x.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
