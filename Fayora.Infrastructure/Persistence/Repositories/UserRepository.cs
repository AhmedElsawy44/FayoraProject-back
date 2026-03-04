using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public void AddUser(User user) => Add(user);

    public async Task<User?> GetUserByIdAsync(Guid id, UserQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsQueryable();

        if (options is null)
        {
            return await query.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        if (!options.IsTracking)
        {
            query = query.AsNoTracking();
        }

        if (options.IncludeVerificationCodes)
        {
            query = query.Include(u => u.VerificationCodes);
        }

        if (options.IncludeResetTokens)
        {
            query = query.Include(u => u.PasswordResetTokens);
        }

        if (options.Status is AccountStatus.Verified)
        {
            query = query.Where(u => (u.IsEmailVerified && u.PrimaryEmail != null) || (u.IsPhoneVerified && u.PhoneNumber != null));
        }
        else if (options.Status is AccountStatus.NotVerified)
        {
            query = query.Where(u => (!u.IsEmailVerified && u.PrimaryEmail != null) || (!u.IsPhoneVerified && u.PhoneNumber != null));
        }

        return await query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByIdentityAsync(
    string identity,
    UserQueryOptions? options = null,
    CancellationToken cancellationToken = default)
    {
        options ??= new UserQueryOptions();

        var query = context.Users.AsQueryable();

        if (!options.IsTracking)
        {
            query = query.AsNoTracking();
        }

        if (options.IncludeResetTokens)
        {
            query = query.Include(u => u.PasswordResetTokens);
        }

        if(options.IncludeVerificationCodes)
        {
            query = query.Include(u => u.VerificationCodes);
        }

        if (identity.Contains('@'))
        {
            var emailObj = Email.Create(identity).Value;

            query = options.Status switch
            {
                AccountStatus.Verified => query.Where(u => u.PrimaryEmail != null && u.IsEmailVerified && u.PrimaryEmail == emailObj),
                AccountStatus.NotVerified => query.Where(u => u.PrimaryEmail != null && !u.IsEmailVerified && u.PrimaryEmail == emailObj),
                _ => query.Where(u => u.PrimaryEmail != null && u.PrimaryEmail == emailObj)
            };
        }
        else
        {
            query = options.Status switch
            {
                AccountStatus.Verified => query.Where(u => u.PhoneNumber != null && u.IsPhoneVerified && u.PhoneNumber == identity),
                AccountStatus.NotVerified => query.Where(u => u.PhoneNumber != null && !u.IsPhoneVerified && u.PhoneNumber == identity),
                _ => query.Where(u => u.PhoneNumber != null && u.PhoneNumber == identity)
            };
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}