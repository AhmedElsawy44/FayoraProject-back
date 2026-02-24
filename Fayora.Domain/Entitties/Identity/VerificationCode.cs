using ErrorOr;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Entitties.Identity;

public class VerificationCode : BaseEntity<int>
{
    public string Target { get; init; }
    public string CodeHash { get; init; }
    public CodeType Type { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsUsed { get; private set; } = false;
    public int AttemptCount { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public ErrorOr<Success> Use(string codeHash)
    {
        if(codeHash != CodeHash)
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
