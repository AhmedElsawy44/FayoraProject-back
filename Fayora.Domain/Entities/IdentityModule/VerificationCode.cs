using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Domain.Entities.IdentityModule;

public class VerificationCode : BaseEntity<int>
{
    public static readonly int MaxAllowedAttempts = 3;
    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public Guid UserId { get; init; }
    public string Target { get; init; } = string.Empty;
    public string CodeHash { get; init; } = string.Empty;
    public CodePurpose Purpose { get; init; } = CodePurpose.VerifyAccount;
    public DateTimeOffset ExpiresAt { get; init; } = DateTimeOffset.UtcNow.Add(DefaultExpiration);
    public DateTimeOffset? RevokedAt { get; private set; } = null;
    public int AttemptCount { get; private set; } = 0;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsValid => !RevokedAt.HasValue && DateTimeOffset.UtcNow < ExpiresAt && AttemptCount < MaxAllowedAttempts;

    public Result<Success> Use(string plainCode, ICodeHasher codeHasher)
    {

        if (!IsValid)
            return Error.Failure(
                "VerificationCode.InvalidOrExpired",
                "This code is invalid or has expired.");

        if (!codeHasher.VerifyCode(plainCode, this.CodeHash))
        {
            AttemptCount++;
            return Error.Failure("VerificationCode.InvalidCode", "The provided code is incorrect.");
        }

        Revoke();
        return Result.Success;
    }

    internal void Revoke() => RevokedAt = DateTimeOffset.UtcNow;

    internal static VerificationCode Create(Guid userId, string target, string codeHash, CodePurpose purpose)
    {
        return new VerificationCode
        {
            UserId = userId,
            Target = target,
            CodeHash = codeHash,
            Purpose = purpose,
            ExpiresAt = DateTimeOffset.UtcNow.Add(DefaultExpiration)
        };
    }

    private VerificationCode() { }
}
