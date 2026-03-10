using Fayora.Domain.Entitties.Tourist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.ItemType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.TouristId).IsRequired();
        builder.Property(w => w.ItemId).IsRequired();
        builder.Property(w => w.CreateAt).IsRequired();

        builder.HasIndex(w => new { w.TouristId, w.ItemType, w.ItemId })
            .IsUnique()
            .HasDatabaseName("IX_Wishlists_UniqueTouristItem");

        builder.HasIndex(w => new { w.TouristId, w.ItemType })
            .HasDatabaseName("IX_Wishlists_Tourist_ItemType");
    }
}