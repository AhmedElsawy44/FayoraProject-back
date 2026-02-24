using System.Net.Http.Json;
using ErrorOr;
using Fayora.Domain.Common;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entitties.Identity;

public class User : AuditableEntity<Guid>
{
    private string _passwordHash = string.Empty;

    private User()
    {
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
    public Gender? Gender { get; private set; }

    public Email? PrimaryEmail { get; private set; }
    public string? PhoneNumber { get; private set; }

    public DateTimeOffset? PasswordChangedAt { get; private set; }

    public DateTimeOffset? LastOtpSentAt { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }

    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }

    public UserStatus Status { get; private set; } = UserStatus.Active;

    public decimal CurrentBalance { get; private set; } = 0;

    public string? NationalityCode { get; private set; }
    public string? SimCountryIsoCode { get; private set; }

    public string PreferredLanguage { get; private set; } = string.Empty;
    public string TimeZone { get; private set; } = string.Empty;

    public string? ProfileImageUrl { get; private set; }
    public string? Description { get; private set; }

    public DateTimeOffset? LastLogin { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsProfileComplete { get; private set; }

    public int ViolationCount { get; private set; }
    public DateTimeOffset? LastViolationDate { get; private set; }

    private readonly List<UserIdentity> _userIdentities = new();
    public IReadOnlyCollection<UserIdentity> UserIdentities => _userIdentities.AsReadOnly();

    public static ErrorOr<User> Create(
        string firstName,
        string lastName,
        string? email,
        string? phoneNumber,
        string passwordHash,
        string nationalityCode,
        string? simCountryIsoCode,
        string preferredLanguage,
        string timeZone)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phoneNumber))
            return UserErrors.EmailOrPhoneRequired;

        Email? validEmail = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsError) return emailResult.Errors;
            validEmail = emailResult.Value;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            PrimaryEmail = validEmail,
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
            _passwordHash = passwordHash,
            Status = UserStatus.Active,
            NationalityCode = nationalityCode,
            SimCountryIsoCode = string.IsNullOrWhiteSpace(simCountryIsoCode) ? null : simCountryIsoCode,
            PreferredLanguage = preferredLanguage,
            TimeZone = timeZone
        };

        return user;
    }

    public ErrorOr<Success> VerifyEmail()
    {
        if (PrimaryEmail == null) return UserErrors.EmailNotProvided;

        if (IsEmailVerified) return Result.Success;

        IsEmailVerified = true;
        return Result.Success;
    }

    public ErrorOr<Success> VerifyPhone()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber)) return UserErrors.PhoneNotProvided;

        if (IsPhoneVerified) return Result.Success;

        IsPhoneVerified = true;
        return Result.Success;
    }

    public ErrorOr<Success> ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) return UserErrors.InvalidPassword;
        _passwordHash = newPasswordHash;
        PasswordChangedAt = DateTimeOffset.UtcNow;
        Updated();
        return Result.Success;
    }

    public ErrorOr<Success> ChangeEmail(string email)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return emailResult.Errors;

        PrimaryEmail = emailResult.Value;
        IsEmailVerified = false;
        Updated();
        return Result.Success;
    }

    public ErrorOr<Success> ChangePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return UserErrors.InvalidPhone;
        PhoneNumber = phoneNumber;
        IsPhoneVerified = false;
        Updated();
        return Result.Success;
    }

    public ErrorOr<Success> LockAccount(TimeSpan lockDuration)
    {
        if (lockDuration <= TimeSpan.Zero) return UserErrors.InvalidLockDuration;
        LockedUntil = DateTimeOffset.UtcNow.Add(lockDuration);
        Status = UserStatus.Locked;
        return Result.Success;
    }

    public void UnlockAccount()
    {
        LockedUntil = null;
        Status = UserStatus.Active;
    }

    public ErrorOr<Success> IncrementViolation()
    {
        ViolationCount++;
        LastViolationDate = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    public ErrorOr<Success> ResetViolations()
    {
        ViolationCount = 0;
        LastViolationDate = null;
        return Result.Success;
    }

    public ErrorOr<Success> MarkAsDeleted()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        Status = UserStatus.Deleted;
        return Result.Success;
    }

    public ErrorOr<Success> UpdateProfile(
        string? firstName,
        string? lastName,
        DateOnly? birthDate,
        Gender? gender,
        string? nationalityCode,
        string? profileImageUrl,
        string? description,
        string? preferredLanguage,
        string? timeZone)
    {
        if (!string.IsNullOrWhiteSpace(firstName)) FirstName = firstName;
        if (!string.IsNullOrWhiteSpace(lastName)) LastName = lastName;
        if (birthDate.HasValue) BirthDate = birthDate;
        if (gender.HasValue) Gender = gender;
        if (!string.IsNullOrWhiteSpace(nationalityCode)) NationalityCode = nationalityCode;
        if (!string.IsNullOrWhiteSpace(profileImageUrl)) ProfileImageUrl = profileImageUrl;
        if (!string.IsNullOrWhiteSpace(preferredLanguage)) PreferredLanguage = preferredLanguage;
        if (!string.IsNullOrWhiteSpace(timeZone)) TimeZone = timeZone;

        Description = description;

        IsProfileComplete = CheckIfProfileComplete();
        Updated();
        return Result.Success;
    }

    private bool CheckIfProfileComplete()
    {
        return !string.IsNullOrWhiteSpace(FirstName)
               && !string.IsNullOrWhiteSpace(LastName)
               && !string.IsNullOrWhiteSpace(PreferredLanguage)
               && BirthDate.HasValue
               && Gender.HasValue
               && (PrimaryEmail != null || !string.IsNullOrWhiteSpace(PhoneNumber))
               && !string.IsNullOrWhiteSpace(NationalityCode)
               && !string.IsNullOrWhiteSpace(ProfileImageUrl);
    }

    public void DeleteImage()
    {
        ProfileImageUrl = null;
        Updated();
    }

    public ErrorOr<Success> AddOrUpdateUserIdentity(
    IdentityProvider provider,
    string providerKey,
    string email,
    string profileDataJson)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return emailResult.Errors;

        var existingIdentity = _userIdentities.FirstOrDefault(i =>
            i.Provider == provider && i.ProviderKey == providerKey);

        if (existingIdentity != null)
        {
            existingIdentity.UpdateProfile(emailResult.Value, profileDataJson);
        }
        else
        {
            var newIdentity = new UserIdentity(Id, provider, providerKey, emailResult.Value, profileDataJson);
            _userIdentities.Add(newIdentity);
        }
        Updated();
        return Result.Success;
    }

    public void RecordLogin()
    {
        LastLogin = DateTimeOffset.UtcNow;
        Updated();
    }


    public void UpdateBalance(decimal amount)
    {
        CurrentBalance += amount;
        Updated();
    }

    public void SendOtp()
    {
        LastOtpSentAt = DateTimeOffset.UtcNow;
        Updated();

    }
}