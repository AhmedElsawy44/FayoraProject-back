using Fayora.Domain.Entities.Identity;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations;

internal class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.NationalityCode).HasMaxLength(10);
        builder.Property(u => u.SimCountryIsoCode).HasMaxLength(10);
        builder.Property(u => u.PreferredLanguage).HasMaxLength(20);
        builder.Property(u => u.TimeZone).HasMaxLength(50);
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(2048);
        builder.Property(u => u.Description).HasMaxLength(1000);

        builder.Property(u => u.BirthDate).HasColumnType("date");

        builder.Property(u => u.PrimaryEmail)
            .HasConversion(
                email => email != null ? email.Value : null,
                value => value != null ? Email.Create(value).Value : null)
            .HasColumnName("Email")
            .HasMaxLength(255);


        builder.Property<string>("_passwordHash")
            .HasColumnName("PasswordHash")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.CurrentBalance)
            .HasColumnType("decimal(18, 4)");

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.HasIndex(u => u.PrimaryEmail)
            .IsUnique().HasFilter("[Email] IS NOT NULL");

        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique().HasFilter("[PhoneNumber IS NOT NULL");

        builder.HasMany(u => u.UserIdentities)
            .WithOne()
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(User.UserIdentities))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}