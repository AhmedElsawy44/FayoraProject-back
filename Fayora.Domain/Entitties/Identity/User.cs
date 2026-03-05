using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common;
using Fayora.Domain.Common.Events;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using Fayora.Domain.Interfaces;
using Fayora.Domain.Policies;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.Identity;

public class User : AuditableEntity<Guid>
{
    public static readonly int MaxUserIdentities = 2;
    public static readonly int MaxVerificationCodesPerDay = 5;
    public static readonly TimeSpan OtpResendCooldown = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan PasswordResetTokenExpiration = TimeSpan.FromMinutes(15);

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
    public string PreferredLanguage { get; private set; } = string.Empty;
    public string TimeZone { get; private set; } = string.Empty;
    public string? ProfileImageUrl { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset? LastLogin { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public bool IsProfileComplete { get; private set; }
    public int ViolationCount { get; private set; }
    public DateTimeOffset? LastViolationDate { get; private set; }

    private readonly List<UserIdentity> _userIdentities = [];
    public IReadOnlyCollection<UserIdentity> UserIdentities => _userIdentities.AsReadOnly();

    private readonly List<VerificationCode> _verificationCodes = [];
    public IReadOnlyCollection<VerificationCode> VerificationCodes => _verificationCodes.AsReadOnly();

    private readonly List<PasswordResetToken> _passwordResetTokens = [];
    public IReadOnlyCollection<PasswordResetToken> PasswordResetTokens => _passwordResetTokens.AsReadOnly();

    private string _passwordHash = string.Empty;

    public static Result<User> Create(
        string? email,
        string? phoneNumber,
        string password,
        string? simCountryIsoCode,
        string preferredLanguage,
        string timeZone,
        IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phoneNumber))
            return UserErrors.EmailOrPhoneRequired;

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(phoneNumber))
            return UserErrors.OnlyOneAllowed;

        var passwordHashResult = passwordHasher.HashPassword(password);
        if (passwordHashResult.IsError) return passwordHashResult.Errors;

        Email? validEmail = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsError) return emailResult.Errors;
            validEmail = emailResult.Value;
        }

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            PrimaryEmail = validEmail,
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
            _passwordHash = passwordHashResult.Value,
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

    internal void IncreaseViolationInternal()
    {

        ViolationCount++;
        LastViolationDate = DateTimeOffset.UtcNow;
    }

    public void IncrementViolation()
    {
        ViolationPolicy.Apply(this);
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
        UpdateProfileImage(profileImageUrl);

        Description = description;
        PreferredLanguage = preferredLanguage ?? PreferredLanguage;
        TimeZone = timeZone ?? TimeZone;

        IsProfileComplete = CheckIfProfileComplete();
        Updated();
    }

    private void UpdateProfileImage(string? profileImageUrl)
    {
        if (profileImageUrl != ProfileImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(ProfileImageUrl))
                RaiseDomainEvent(new DeleteMediaEvent(ProfileImageUrl));
            else
                DeleteProfileImage();

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

    public void DeleteProfileImage()
    {
        if (ProfileImageUrl != null)
        {
            RaiseDomainEvent(new DeleteMediaEvent(ProfileImageUrl));
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
            if (_userIdentities.Count >= MaxUserIdentities) return UserErrors.TooManyIdentities;
            var newIdentity = new UserIdentity(Id, provider, providerKey, emailResult.Value, profileDataJson);
            _userIdentities.Add(newIdentity);
        }
        return Result.Success;
    }

    public void RecordLogin()
    {
        LastLogin = DateTimeOffset.UtcNow;
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

    public Result<Success> RequestOtp(
    string target,
    OtpPurpose purpose,
    IVerificationCodeService codeService,
    ICodeHasher codeHasher)
    {
        var validationResult = ValidateOtpRequest(target, purpose);
        if (validationResult.IsError)
            return validationResult.Errors;

        RevokeActiveCodes(purpose);

        var (rawCode, verificationCode) = CreateVerificationCode(
            target,
            purpose,
            codeService,
            codeHasher);

        _verificationCodes.Add(verificationCode);
        LastOtpSentAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new OtpRequestedDomainEvent(Id, target, rawCode, purpose));

        return Result.Success;
    }

    private Result<Success> ValidateOtpRequest(string target, OtpPurpose purpose)
    {
        if (LastOtpSentAt.HasValue &&
            DateTimeOffset.UtcNow < LastOtpSentAt.Value.Add(OtpResendCooldown))
            return UserErrors.OtpCooldownNotMet;

        var last24hCount = _verificationCodes
            .Count(c => c.CreatedAt > DateTimeOffset.UtcNow.AddDays(-1));

        if (last24hCount >= MaxVerificationCodesPerDay)
            return UserErrors.DailyOtpLimitReached;

        if (target != PrimaryEmail?.Value && target != PhoneNumber)
            return UserErrors.InvalidTarget;


        return Result.Success;
    }

    private void RevokeActiveCodes(OtpPurpose purpose)
    {
        foreach (var code in _verificationCodes
                     .Where(c => c.Purpose == purpose && !c.IsRevoked))
        {
            code.Revoke();
        }
    }

    private (string rawCode, VerificationCode code) CreateVerificationCode(
    string target,
    OtpPurpose purpose,
    IVerificationCodeService codeService,
    ICodeHasher codeHasher)
    {
        var rawCode = codeService.GenerateCode();
        var hashed = codeHasher.HashCode(rawCode);

        var verificationCode =
            VerificationCode.Create(Id, target, hashed, purpose);

        return (rawCode, verificationCode);
    }

    public string GeneratePasswordResetToken(ICodeHasher codeHasher)
    {
        var activeTokens = _passwordResetTokens.Where(t => t.IsValid);
        foreach (var token in activeTokens)
        {
            token.Revoke();
        }

        string rawToken = Guid.CreateVersion7().ToString("N");
        string tokenHash = codeHasher.HashCode(rawToken);

        var newToken = PasswordResetToken.Create(Id, tokenHash, PasswordResetTokenExpiration);
        _passwordResetTokens.Add(newToken);

        return rawToken;
    }

    public Result<Success> ResetPassword(string rawToken, string newPassword, ICodeHasher codeHasher, IPasswordHasher passwordHasher)
    {
        var providedTokenHash = codeHasher.HashCode(rawToken);

        var resetToken = _passwordResetTokens.FirstOrDefault(p => p.IsValid && codeHasher.VerifyCode(rawToken, p.TokenHash));

        if (resetToken == null)
            return UserErrors.InvalidResetToken;

        var passwordHashResult = passwordHasher.HashPassword(newPassword);
        if (passwordHashResult.IsError) return passwordHashResult.Errors;

        resetToken.Consume();

        _passwordHash = passwordHashResult.Value;
        PasswordChangedAt = DateTimeOffset.UtcNow;

        Updated();

        if (PrimaryEmail != null)
            RaiseDomainEvent(new PasswordResetedEvent(Id, PrimaryEmail.Value));

        return Result.Success;
    }

    public Result<Success> VerifyOtp(string target, string otp, OtpPurpose purpose, ICodeHasher codeHasher)
    {
        var code = _verificationCodes.FirstOrDefault(c =>
            c.Target == target &&
            c.Purpose == purpose
            && c.IsValid);

        if (code is null)
            return UserErrors.CodeNotFound;

        var useResult = code.Use(otp, codeHasher);

        if (useResult.IsError)
            return useResult.Errors;

        ApplyStateChangesBasedOnPurpose(target, purpose);

        return Result.Success;
    }

    private void ApplyStateChangesBasedOnPurpose(string target, OtpPurpose purpose)
    {
        switch (purpose)
        {
            case OtpPurpose.Registration:
                if (PrimaryEmail?.Value == target)
                {
                    VerifyEmail();
                }
                else if (PhoneNumber == target)
                {
                    VerifyPhone();
                }

                RaiseDomainEvent(new UserVerifiedDomainEvent(Id, target));
                break;
        }
    }

    private User() { }
}