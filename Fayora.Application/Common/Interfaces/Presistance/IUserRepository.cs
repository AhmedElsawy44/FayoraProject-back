using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserRepository
{
    void AddUser(User user);
    Task<User?> GetUserByIdAsync(Guid id, UserQueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdentityAsync(
        string identity,
        UserQueryOptions? options = null,
        CancellationToken cancellationToken = default);

    public enum AccountStatus
    {
        Verified,
        NotVerified,
        All
    }

    public record UserQueryOptions(
        AccountStatus Status = AccountStatus.All,
        bool IsTracking = false,
        bool IncludeResetTokens = false,
        bool IncludeVerificationCodes = false
    );
}
