using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IUserRepository
{
    void AddUser(User user);

    Task<User?> GetUserByIdAsync(Guid id, UserQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, UserQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<User?> GetUserByPhoneAsync(string phoneNumber, UserQueryOptions? options = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, CancellationToken cancellationToken = default);

    public enum AccountStatus
    {
        Verified,
        NotVerified,
        All
    }

    public record UserQueryOptions(
        bool IsReadOnly = false,
        bool IncludeVerificationCodes = false,
        AccountStatus Status = AccountStatus.All,
        UserStatus? UserStatus = null
    );
}