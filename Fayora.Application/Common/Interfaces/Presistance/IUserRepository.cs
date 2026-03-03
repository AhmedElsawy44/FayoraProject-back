using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserRepository
{
    Task<bool> IsIdentityExistAsync(string identity, IdentityType type, CancellationToken cancellationToken, AccountStatus status = AccountStatus.All);
    void AddUser(User user);
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken, bool isTracking = false);

    public enum AccountStatus
    {
        Verified,
        NotVerified,
        All
    }

    public enum IdentityType
    {
        Email,
        Phone
    }
}
