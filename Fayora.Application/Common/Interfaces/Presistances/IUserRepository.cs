using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IUserRepository
{
    void AddUser(User user);

    Task<User?> GetUserByIdAsync(Guid id, UserQueryOptions? options = null, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdentityAsync(string identity, UserQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<bool> IsBannedAsync(string identity, CancellationToken cancellationToken = default);

    public enum AccountStatus
    {
        Verified,
        NotVerified,
        All
    }

    public record UserQueryOptions(
        bool IsTracking = false,
        bool IncludeVerificationCodes = false,
        bool IncludeRoles = false,
        AccountStatus Status = AccountStatus.All,
        UserStatus? UserStatus = null
    );
}