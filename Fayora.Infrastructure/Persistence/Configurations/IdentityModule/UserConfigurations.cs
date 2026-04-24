using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fayora.Infrastructure.Persistence.Configurations.IdentityModule;

internal class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnType("NVARCHAR(50)")
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .HasColumnType("DATE");

        builder.Property(u => u.Gender)
            .HasConversion<int>();

        builder.Property(u => u.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.CurrentBalance)
            .HasColumnType("DECIMAL(18, 4)")
            .IsRequired();

        builder.Property(u => u.SimCountryIsoCode)
            .HasMaxLength(10);

        builder.Property(u => u.PreferredLanguage)
            .HasConversion<int>();

        builder.Property(u => u.TimeZone)
            .HasMaxLength(50);

        builder.Property(u => u.Description)
            .HasColumnType("NVARCHAR(1000)");

        builder.Property(u => u.NationalityCode)
            .HasColumnType("NVARCHAR(3)")
            .HasMaxLength(3);

        builder.Property<string>("_passwordHash")
            .HasField("_passwordHash")
            .HasColumnName("PasswordHash")
            .HasMaxLength(256)
            .IsRequired();


        builder.Property(u => u.PrimaryEmail)
            .HasConversion(
                email => email != null ? email.Value : null,
                value => value != null ? Email.Create(value).Value : null)
            .HasColumnName("Email")
            .HasMaxLength(255);

        builder.HasIndex(u => u.PrimaryEmail)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");


        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                phone => phone != null ? phone.Value : null,
                value => value != null ? PhoneNumber.Create(value).Value : null)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(20);

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique()
            .HasFilter("[PhoneNumber] IS NOT NULL");


        builder.Property(u => u.ProfileImageUrl)
            .HasConversion(
                url => url != null ? url.ToString() : null,
                value => value != null ? FileUrl.Create(value).Value : null)
            .HasColumnName("ProfileImageUrl")
            .HasColumnType("NVARCHAR(2048)");

        builder.Ignore(u => u.SpokenLanguages);

        builder.Property<List<UserLanguageProficiency>>("_userLanguageProficiencies")
            .HasField("_userLanguageProficiencies")
            .HasColumnName("UserLanguageProficiencies")
            .HasColumnType("NVARCHAR(MAX)")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<UserLanguageProficiency>()
                    : JsonSerializer.Deserialize<List<UserLanguageProficiency>>(v, (JsonSerializerOptions?)null)
                      ?? new List<UserLanguageProficiency>()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<UserLanguageProficiency>>(
                (c1, c2) => (c1 ?? new List<UserLanguageProficiency>()).SequenceEqual(c2 ?? new List<UserLanguageProficiency>()),
                c => (c ?? new List<UserLanguageProficiency>()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (c ?? new List<UserLanguageProficiency>()).ToList()
            ));

        builder.Ignore(u => u.UserLanguageProficiencies);

        builder.HasMany(u => u.VerificationCodes)
            .WithOne()
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(u => u.VerificationCodes)
            .HasField("_verificationCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<UserDevice>()
            .WithOne()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}