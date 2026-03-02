using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Entities.Identity;

public class VerificationCode : BaseEntity<int>
{
    public static readonly int MaxAllowedAttempts = 3;
    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(15);

    public Guid UserId { get; init; }
    public string Target { get; init; } = string.Empty;
    public string CodeHash { get; init; } = string.Empty;
    public OtpPurpose Purpose { get; init; } = OtpPurpose.Registration;
    public DateTimeOffset ExpiresAt { get; init; } = DateTimeOffset.UtcNow.Add(DefaultExpiration);
    public bool IsUsed { get; private set; } = false;
    public int AttemptCount { get; private set; } = 0;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
    public bool IsBlocked => AttemptCount >= MaxAllowedAttempts;
    public bool IsEmailType => Target.Contains('@');
    public bool IsSmsType => !IsEmailType && System.Text.RegularExpressions.Regex.IsMatch(Target, @"^\+?[0-9]{10,15}$");

    public Result<Success> Use(string plainCode, ICodeHasher codeHasher)
    {
        if (IsUsed)
            return Error.Failure("VerificationCode.AlreadyUsed", "This code has already been used.");

        if (DateTime.UtcNow > ExpiresAt)
            return Error.Failure("VerificationCode.Expired", "This code has expired.");

        if (!codeHasher.VerifyCode(plainCode, this.CodeHash))
        {
            AttemptCount++;
            return Error.Failure("VerificationCode.InvalidCode", "The provided code is incorrect.");
        }

        IsUsed = true;
        return Result.Success;
    }

    internal static VerificationCode Create(Guid userId, string target, string codeHash, OtpPurpose purpose)
    {
        return new VerificationCode
        {
            UserId = userId,
            Target = target,
            CodeHash = codeHash,
            Purpose = purpose,
        };
    }

    private VerificationCode() { }
}
