using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Entities.TouristModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.TouristModule
{
    public class UserInteractionConfiguration : IEntityTypeConfiguration<UserInteraction>
    {
        public void Configure(EntityTypeBuilder<UserInteraction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityType)
                   .HasConversion<int>();

            builder.Property(x => x.InteractionType)
                   .HasConversion<int>();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
