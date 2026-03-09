using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Events;
using Fayora.Domain.Common.Events.IdentityModule;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;
using Fayora.Domain.Shared.IdentityModule;
using Fayora.Domain.Shared.TouristModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entitties.Identity;

public class User : AuditableEntity<Guid>
{
    public static readonly int MaxSMSOtpPerDay = 5;
    public static readonly int MaxEmailOtpPerDay = 10;
    public static readonly TimeSpan OtpResendCooldown = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan PasswordResetTokenExpiration = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan AccountLockoutDuration = TimeSpan.FromMinutes(15);
    public static readonly int MaxFailedAccessAttempts = 5;
    public static readonly TimeSpan FailedAccessAttemptWindow = TimeSpan.FromMinutes(15);

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
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
    public string? PreferredLanguage { get; private set; } = string.Empty;
    public string? TimeZone { get; private set; } = string.Empty;
    public string? ProfileImageUrl { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset? LastLogin { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public DateTimeOffset? LastFailedLoginAt { get; private set; }
    public bool IsProfileComplete { get; private set; }
    public int ViolationCount { get; private set; }
    public int AccessFailedCount { get; private set; }
    public DateTimeOffset? LastViolationDate { get; private set; }

    private readonly List<VerificationCode> _verificationCodes = [];
    public IReadOnlyCollection<VerificationCode> VerificationCodes => _verificationCodes.AsReadOnly();

    private readonly List<UserRole> _roles = [];
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public List<string> GetRoleNames() => Roles.Select(r => r.Role.Name).ToList();

    private string _passwordHash = string.Empty;

    public bool IsVerified => IsEmailVerified || IsPhoneVerified;
    public bool IsLocked => Status == UserStatus.Locked && LockedUntil.HasValue && LockedUntil.Value > DateTimeOffset.UtcNow;
    public bool IsDeleted => Status == UserStatus.Deleted && DeletedAt.HasValue;
    public bool IsBanned => Status == UserStatus.Banned;

    public static Result<User> CreateWithEmail(string email, string password, IPasswordHasher passwordHasher)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return emailResult.Errors;

        var passwordHashResult = passwordHasher.HashPassword(password);
        if (passwordHashResult.IsError) return passwordHashResult.Errors;

        return new User
        {
            Id = Guid.CreateVersion7(),
            PrimaryEmail = emailResult.Value,
            PhoneNumber = null,
            _passwordHash = passwordHashResult.Value,
            Status = UserStatus.Active,
        };
    }

    public static Result<User> CreateWithPhone(string phoneNumber, string password, IPasswordHasher passwordHasher)
    {
        var passwordHashResult = passwordHasher.HashPassword(password);
        if (passwordHashResult.IsError) return passwordHashResult.Errors;

        return new User
        {
            Id = Guid.CreateVersion7(),
            PrimaryEmail = null,
            PhoneNumber = phoneNumber,
            _passwordHash = passwordHashResult.Value,
            Status = UserStatus.Active,
        };
    }


    public static User CreateWithSocialLogin(
    string? email,
    string? name,
    string? pictureUrl)
    {
        var names = name?.Split(' ');

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            IsEmailVerified = !string.IsNullOrWhiteSpace(email),
            ProfileImageUrl = pictureUrl,
            FirstName = names?.FirstOrDefault(),
            LastName = names?.LastOrDefault(),
            Status = UserStatus.Active,
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsSuccess)
                user.PrimaryEmail = emailResult.Value;
        }

        return user;
    }

    public static User CreateWithSocialLogin(string? email)
    {
        return new User
        {
            Id = Guid.CreateVersion7(),
            PrimaryEmail = Email.Create(email).Value,
            Status = UserStatus.Active,
        };
    }

    public static User CreateWithSocialLogin(
    string? email,
    string? firstName,
    string? lastName,
    string? pictureUrl)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            IsEmailVerified = !string.IsNullOrWhiteSpace(email),
            ProfileImageUrl = pictureUrl,
            FirstName = firstName,
            LastName = lastName,
            Status = UserStatus.Active,
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsSuccess)
                user.PrimaryEmail = emailResult.Value;
        }

        return user;
    }


    public void UpdateRegionalPreferences(string? simCountryIso, string? preferredLanguage, string? timeZone)
    {
        SimCountryIsoCode = simCountryIso ?? SimCountryIsoCode;
        PreferredLanguage = preferredLanguage ?? PreferredLanguage;
        TimeZone = timeZone ?? TimeZone;
    }

    public void Login() => LastLogin = DateTimeOffset.UtcNow;

    private void RecordPasswordFailure()
    {
        var now = DateTimeOffset.UtcNow;

        if (LastFailedLoginAt.HasValue && now > LastFailedLoginAt.Value.Add(FailedAccessAttemptWindow))
        {
            AccessFailedCount = 1;
        }
        else
        {
            AccessFailedCount++;
        }

        LastFailedLoginAt = now;

        if (AccessFailedCount >= MaxFailedAccessAttempts)
        {
            LockAccount();
        }
    }

    private void ResetAccessStats()
    {
        AccessFailedCount = 0;
        LockedUntil = null;
        LastFailedLoginAt = null;
    }

    private void LockAccount() => LockedUntil = DateTimeOffset.UtcNow.Add(AccountLockoutDuration);

    public bool IsCorrectPasswordHash(string password, IPasswordHasher passwordHasher)
    {
        if (passwordHasher.VerifyPassword(password, _passwordHash))
        {
            ResetAccessStats();
            return true;
        }
        else
        {
            RecordPasswordFailure();
            return false;
        }
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        RaiseDomainEvent(new UserSecurityActivityDomainEvent(Id, PrimaryEmail!.Value, SecurityActivityType.EmailVerified));
    }

    public void VerifyPhone() => IsPhoneVerified = true;

    public Result<Success> ChangeEmail(string email)
    {
        var emailResult = Email.Create(email);

        if (emailResult.IsError) return emailResult.Errors;

        var newEmail = emailResult.Value;

        if (PrimaryEmail is not null && PrimaryEmail == newEmail)
            return Result.Success;

        PrimaryEmail = newEmail;
        IsEmailVerified = false;

        Updated();

        RaiseDomainEvent(new UserSecurityActivityDomainEvent(Id, PrimaryEmail.Value, SecurityActivityType.EmailChanged));

        return Result.Success;
    }

    public void ChangePhoneNumber(string phoneNumber)
    {
        if (PhoneNumber == phoneNumber)
            return;

        PhoneNumber = phoneNumber;
        IsPhoneVerified = false;

        Updated();
    }

    public void Delete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        Status = UserStatus.Deleted;
        if (PrimaryEmail is not null)
            RaiseDomainEvent(new UserSecurityActivityDomainEvent(Id, PrimaryEmail.Value, SecurityActivityType.AccountDeleted));
    }

    public void Restore()
    {
        DeletedAt = null;
        Status = UserStatus.Active;
        IsEmailVerified = false;
        IsPhoneVerified = false;
        Updated();
        if (PrimaryEmail is not null)
            RaiseDomainEvent(new UserSecurityActivityDomainEvent(Id, PrimaryEmail.Value, SecurityActivityType.AccountRestored));
    }


    public void UpdateProfile(string? firstName, string? lastName, DateOnly? birthDate, Gender? gender, string? nationalityCode, string? profileImageUrl, string? description, string? preferredLanguage, string? timeZone)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Gender = gender;
        NationalityCode = nationalityCode;

        UpdateProfileImage(profileImageUrl);

        Description = description;
        PreferredLanguage = preferredLanguage;
        TimeZone = timeZone;
        IsProfileComplete = CheckIfProfileComplete();

        Updated();
    }

    private void UpdateProfileImage(string? profileImageUrl)
    {
        if (profileImageUrl != ProfileImageUrl)
        {
            if (ProfileImageUrl is not null)
                RaiseDomainEvent(new DeleteMediaEvent(ProfileImageUrl));

            ProfileImageUrl = profileImageUrl;
        }
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

    public Result<Success> Credit(decimal amount)
    {
        if (amount <= 0)
            return UserErrors.InvalidAmount;

        CurrentBalance += amount;
        return Result.Success;
    }

    public Result<Success> Debit(decimal amount)
    {
        if (amount <= 0)
            return UserErrors.InvalidAmount;

        if (CurrentBalance < amount)
            return UserErrors.InsufficientBalance;

        CurrentBalance -= amount;
        return Result.Success;
    }

    public Result<Success> CanRequestPhoneCode()
    {
        if (CheckOtpCooldown())
            return UserErrors.OtpCooldownNotMet;

        if (HasReachedDailyOtpLimit(MaxSMSOtpPerDay))
            return UserErrors.DailyOtpLimitReached;

        return Result.Success;
    }

    public Result<Success> CanRequestEmailCode()
    {
        if (CheckOtpCooldown())
            return UserErrors.OtpCooldownNotMet;

        if (HasReachedDailyOtpLimit(MaxEmailOtpPerDay))
            return UserErrors.DailyOtpLimitReached;

        return Result.Success;
    }

    private bool CheckOtpCooldown() => LastOtpSentAt.HasValue && DateTimeOffset.UtcNow < LastOtpSentAt.Value.Add(OtpResendCooldown);

    private bool HasReachedDailyOtpLimit(int limit)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-1);
        var last24hCount = _verificationCodes.Count(c => c.CreatedAt > cutoff);
        return last24hCount >= limit;
    }

    public Result<Success> ChangePassword(string newPassword, IPasswordHasher passwordHasher)
    {
        var passwordHashResult = passwordHasher.HashPassword(newPassword);
        if (passwordHashResult.IsError) return passwordHashResult.Errors;

        _passwordHash = passwordHashResult.Value;
        PasswordChangedAt = DateTimeOffset.UtcNow;

        Updated();

        if (PrimaryEmail is not null && IsEmailVerified)
            RaiseDomainEvent(new UserSecurityActivityDomainEvent(Id, PrimaryEmail.Value, SecurityActivityType.PasswordReset));

        return Result.Success;
    }

    public void SendEmailCode(string email, string code, CodePurpose purpose, ICodeHasher codeHasher)
    {
        GenerateAndStoreCode(email, code, purpose, codeHasher);
        RaiseDomainEvent(new EmailCodeRequestedEvent(Id, email, code, purpose));
    }

    public void SendPhoneCode(string phoneNumber, string code, CodePurpose purpose, CodeDeliveryMethod deliveryMethod, ICodeHasher codeHasher)
    {
        GenerateAndStoreCode(phoneNumber, code, purpose, codeHasher);

        RaiseDomainEvent(new PhoneCodeRequestedEvent(Id, phoneNumber, code, purpose, deliveryMethod));
    }

    private void GenerateAndStoreCode(string target, string code, CodePurpose purpose, ICodeHasher codeHasher)
    {
        var codeHash = codeHasher.HashCode(code);
        var verificationCode = VerificationCode.Create(Id, target, codeHash, purpose);
        _verificationCodes.Add(verificationCode);

        LastOtpSentAt = DateTimeOffset.UtcNow;
    }

    public Result<Success> CheckActiveStatus()
    {
        if (IsLocked)
            return Error.Failure("User.UserLocked", $"This user account is locked until {LockedUntil?.ToString("u")}.");

        if (IsDeleted)
            return Error.Failure("User.UserDeleted", "This user account has been deleted.");

        if (IsBanned)
            return Error.Failure("User.UserBanned", "This user account has been banned.");

        return Result.Success;
    }

    private User() { }
}