using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Entities.Identity;

public class VerificationCode : BaseEntity<int>
{
    public static readonly int MaxAllowedAttempts = 3;
    public string Target { get; init; }
    public string CodeHash { get; init; }
    public CodeType Type { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsUsed { get; private set; } = false;
    public int AttemptCount { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
    public bool IsBlocked => AttemptCount >= MaxAllowedAttempts;

    public Result<Success> Use(string codeHash)
    {
        if (IsExpired)
            return Error.Validation("VerificationCode.Expired", "This code has expired.");

        if (IsBlocked)
            return Error.Validation("VerificationCode.MaxAttemptsReached", "Maximum verification attempts reached. Please request a new code.");

        if (codeHash != CodeHash)
        {
            AttemptCount++;
            return Error.Failure("VerificationCode.InvalidCode", "The provided code is incorrect.");
        }

        IsUsed = true;
        return Result.Success;
    }

    public VerificationCode (string target, string codeHash, CodeType type, DateTimeOffset expiresAt)
    {
        Target = target;
        CodeHash = codeHash;
        Type = type;
        ExpiresAt = expiresAt;
    }

    private VerificationCode() { }
}
