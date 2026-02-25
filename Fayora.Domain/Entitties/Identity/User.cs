using Fayora.Domain.Common;
using Fayora.Domain.Common.Events;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.Identity;

public class User : AuditableEntity<Guid>
{
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

    private string _passwordHash = string.Empty;

    public static Result<User> Create(
        string firstName,
        string lastName,
        string? email,
        string? phoneNumber,
        string passwordHash,
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
            FirstName = firstName,
            LastName = lastName,
            PrimaryEmail = validEmail,
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
            _passwordHash = passwordHash,
            Status = UserStatus.Active,
            SimCountryIsoCode = simCountryIsoCode,
            PreferredLanguage = preferredLanguage,
            TimeZone = timeZone
        };

        return user;
    }

    public Result<Success> VerifyEmail()
    {
        if (PrimaryEmail == null) return UserErrors.EmailNotProvided;

        if (IsEmailVerified) return Result.Success;

        IsEmailVerified = true;
        return Result.Success;
    }

    public Result<Success> VerifyPhone()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber)) return UserErrors.PhoneNotProvided;

        if (IsPhoneVerified) return Result.Success;

        IsPhoneVerified = true;
        return Result.Success;
    }

    public Result<Success> ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) return UserErrors.InvalidPassword;
        _passwordHash = newPasswordHash;
        PasswordChangedAt = DateTimeOffset.UtcNow;
        Updated();
        return Result.Success;
    }

    public Result<Success> ChangeEmail(string email)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return emailResult.Errors;

        if (PrimaryEmail != null && PrimaryEmail.Value == emailResult.Value.Value)
            return Result.Success;

        PrimaryEmail = emailResult.Value;
        IsEmailVerified = false;
        Updated();
        return Result.Success;
    }

    public Result<Success> ChangePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return UserErrors.InvalidPhone;

        if (PhoneNumber == phoneNumber)
            return Result.Success;

        PhoneNumber = phoneNumber;
        IsPhoneVerified = false;
        Updated();
        return Result.Success;
    }

    private void LockAccount(TimeSpan lockDuration)
    {
        LockedUntil = DateTimeOffset.UtcNow.Add(lockDuration);
        Status = UserStatus.Locked;
    }

    public void UnlockAccount()
    {
        LockedUntil = null;
        Status = UserStatus.Active;
    }

    public void IncrementViolation()
    {
        if (LastViolationDate.HasValue && DateTimeOffset.UtcNow > LastViolationDate.Value.AddDays(30))
        {
            ViolationCount = 0;
        }

        ViolationCount++;
        LastViolationDate = DateTimeOffset.UtcNow;

        switch (ViolationCount)
        {
            case >= 5:
                LockAccount(TimeSpan.FromDays(30));
                break;

            case 4:
                LockAccount(TimeSpan.FromDays(7));
                break;

            case 3:
                LockAccount(TimeSpan.FromDays(1));
                break;

            default:
                break;
        }
    }

    public void ResetViolations()
    {
        ViolationCount = 0;
        LastViolationDate = null;
    }

    public void MarkAsDeleted()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        Status = UserStatus.Deleted;
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        DateOnly? birthDate,
        Gender? gender,
        string? nationalityCode,
        string? profileImageUrl,
        string? description,
        string? preferredLanguage,
        string? timeZone)
    {

        FirstName = firstName;
        LastName = lastName;

        BirthDate = birthDate;
        Gender = gender;
        NationalityCode = nationalityCode;
        if (profileImageUrl != ProfileImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(ProfileImageUrl))
            {
                AddDomainEvent(new DeleteMediaEvent(ProfileImageUrl));
            }
            ProfileImageUrl = profileImageUrl;
        }
        Description = description;
        PreferredLanguage = preferredLanguage ?? PreferredLanguage;
        TimeZone = timeZone ?? TimeZone;

        IsProfileComplete = CheckIfProfileComplete();
        Updated();
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
        if (ProfileImageUrl != null)
        {
            AddDomainEvent(new DeleteMediaEvent(ProfileImageUrl));
            ProfileImageUrl = null;
        }
    }

    public Result<Success> AddOrUpdateUserIdentity(
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
            if(_userIdentities.Count >= 2) return UserErrors.TooManyIdentities;
            var newIdentity = new UserIdentity(Id, provider, providerKey, emailResult.Value, profileDataJson);
            _userIdentities.Add(newIdentity);
        }
        return Result.Success;
    }

    public void RecordLogin()
    {
        LastLogin = DateTimeOffset.UtcNow;
    }

    public void UpdateBalance(decimal amount)
    {
        CurrentBalance += amount;
    }

    public void RecordOtpSent()
    {
        LastOtpSentAt = DateTimeOffset.UtcNow;
    }

    private User() { }
}