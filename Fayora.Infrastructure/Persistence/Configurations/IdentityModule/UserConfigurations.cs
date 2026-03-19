using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fayora.Infrastructure.Persistence.Configurations.IdentityModule;

internal class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasColumnType("NVARCHAR(50)");

        builder.Property(u => u.LastName)
            .HasColumnType("NVARCHAR(50)");

        builder.Property(u => u.BirthDate).HasColumnType("DATE");

        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.HasIndex(u => u.PrimaryEmail)
            .IsUnique().HasFilter("[Email] IS NOT NULL");

        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");


        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.CurrentBalance)
            .HasColumnType("DECIMAL(18, 4)");

        builder.Property(u => u.NationalityCode).HasMaxLength(10);

        builder.Property(u => u.SimCountryIsoCode).HasMaxLength(10);

        builder.Property(u => u.PreferredLanguage).HasColumnType("NVARCHAR(20)");

        builder.Property(u => u.TimeZone).HasMaxLength(50);

        builder.Property(u => u.ProfileImageUrl).HasMaxLength(2048);

        builder.Property(u => u.Description).HasColumnType("NVARCHAR(1000)");


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

        builder.HasMany(u => u.VerificationCodes)
            .WithOne()
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(u => u.VerificationCodes)
            .HasField("_verificationCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(u => u.Roles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);


        builder.HasMany<UserDevice>()
            .WithOne()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}