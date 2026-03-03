using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public void AddUser(User user) => Add(user);

    public Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken, bool isTracking = false)
        => GetSingleAsync(u => u.Id == id, cancellationToken, isTracking);

    public Task<bool> IsIdentityExistAsync(string identity, IdentityType type, CancellationToken cancellationToken, AccountStatus status = AccountStatus.All)
    {
        return type switch
        {
            IdentityType.Email => CheckEmailExistsByStatusAsync(identity, status, cancellationToken),
            IdentityType.Phone => CheckPhoneExistsByStatusAsync(identity, status, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported identity type: {type}")
        };
    }

    private Task<bool> CheckEmailExistsByStatusAsync(string identity, AccountStatus status, CancellationToken cancellationToken)
    {
        var email = Email.Create(identity).Value;
        return status switch
        {
            AccountStatus.Verified => IsExistAsync(u => u.PrimaryEmail != null && u.IsEmailVerified && u.PrimaryEmail == email, cancellationToken),
            AccountStatus.NotVerified => IsExistAsync(u => u.PrimaryEmail != null && !u.IsEmailVerified && u.PrimaryEmail == email, cancellationToken),
            _ => IsExistAsync(u => u.PrimaryEmail != null && u.PrimaryEmail == email, cancellationToken)
        };
    }

    private Task<bool> CheckPhoneExistsByStatusAsync(string identity, AccountStatus status, CancellationToken cancellationToken)
    {
        return status switch
        {
            AccountStatus.Verified => IsExistAsync(u => u.PhoneNumber != null && u.IsPhoneVerified && u.PhoneNumber == identity, cancellationToken),
            AccountStatus.NotVerified => IsExistAsync(u => u.PhoneNumber != null && !u.IsPhoneVerified && u.PhoneNumber == identity, cancellationToken),
            _ => IsExistAsync(u => u.PhoneNumber != null && u.PhoneNumber == identity, cancellationToken)
        };
    }
}