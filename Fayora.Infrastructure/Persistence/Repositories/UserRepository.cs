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

    public Task<User?> GetUserByIdentity(string identity, string? simCountryIsoCode, CancellationToken cancellationToken, AccountStatus status = AccountStatus.All, bool isTracking = false)
    {
        if (simCountryIsoCode is null)
        {
            return status switch
            {
                AccountStatus.Verified => GetSingleAsync(u => u.PrimaryEmail != null && u.IsEmailVerified && u.PrimaryEmail == Email.Create(identity).Value, cancellationToken, isTracking),
                AccountStatus.NotVerified => GetSingleAsync(u => u.PrimaryEmail != null && !u.IsEmailVerified && u.PrimaryEmail == Email.Create(identity).Value, cancellationToken, isTracking),
                _ => GetSingleAsync(u => u.PrimaryEmail != null && u.PrimaryEmail == Email.Create(identity).Value, cancellationToken, isTracking)
            };

        }
        else
        {
            return status switch
            {
                AccountStatus.Verified => GetSingleAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && u.IsPhoneVerified && u.PhoneNumber == identity, cancellationToken, isTracking),
                AccountStatus.NotVerified => GetSingleAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && !u.IsPhoneVerified && u.PhoneNumber == identity, cancellationToken, isTracking),
                _ => GetSingleAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && u.PhoneNumber == identity, cancellationToken, isTracking)
            };
        }
    }

    public Task<bool> IsIdentityExistAsync(string identity, string? simCountryIsoCode, CancellationToken cancellationToken, AccountStatus status = AccountStatus.All)
    {
        return simCountryIsoCode switch
        {
            null => CheckEmailExistsByStatusAsync(identity, status, cancellationToken),
            _ => CheckPhoneExistsByStatusAsync(identity, simCountryIsoCode, status, cancellationToken),
        };
    }

    private Task<bool> CheckEmailExistsByStatusAsync(string email, AccountStatus status, CancellationToken cancellationToken)
    {
        var emailObj = Email.Create(email).Value;
        return status switch
        {
            AccountStatus.Verified => IsExistAsync(u => u.PrimaryEmail != null && u.IsEmailVerified && u.PrimaryEmail == emailObj, cancellationToken),
            AccountStatus.NotVerified => IsExistAsync(u => u.PrimaryEmail != null && !u.IsEmailVerified && u.PrimaryEmail == emailObj, cancellationToken),
            _ => IsExistAsync(u => u.PrimaryEmail != null && u.PrimaryEmail == emailObj, cancellationToken)
        };
    }

    private Task<bool> CheckPhoneExistsByStatusAsync(string phoneNumber, string? simCountryIsoCode, AccountStatus status, CancellationToken cancellationToken)
    {
        return status switch
        {
            AccountStatus.Verified => IsExistAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && u.IsPhoneVerified && u.PhoneNumber == phoneNumber, cancellationToken),
            AccountStatus.NotVerified => IsExistAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && !u.IsPhoneVerified && u.PhoneNumber == phoneNumber, cancellationToken),
            _ => IsExistAsync(u => u.PhoneNumber != null && u.SimCountryIsoCode == simCountryIsoCode && u.PhoneNumber == phoneNumber, cancellationToken)
        };
    }
}