using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public void AddUser(User user) => context.Users.Add(user);

    public async Task<User?> GetUserByIdAsync(Guid id, UserQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = context.Users.Where(u => u.Id == id);

        if (options is null)
            return await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        query = ApplyQueryOptions(query, options);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetUserByIdentityAsync(string identity, UserQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new UserQueryOptions();
        var query = context.Users.AsQueryable();

        bool isEmail = identity.Contains('@');

        if (isEmail)
        {
            var emailResult = Email.Create(identity);
            if (emailResult.IsError) return null;

            query = query.Where(u => u.PrimaryEmail == emailResult.Value);
        }
        else
        {
            query = query.Where(u => u.PhoneNumber == identity);
        }

        query = ApplyQueryOptions(query, options, isEmail);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }


    private static IQueryable<User> ApplyQueryOptions(IQueryable<User> query, UserQueryOptions options, bool? isEmailIdentity = null)
    {
        if (!options.IsTracking)
        {
            query = query.AsNoTracking();
        }

        int collectionsIncluded = 0;

        if (options.IncludeVerificationCodes)
        {
            query = query.Include(u => u.VerificationCodes);
            collectionsIncluded++;
        }

        if (options.IncludeRoles)
        {
            query = query.Include(u => u.Roles).ThenInclude(ur => ur.Role);
            collectionsIncluded++;
        }

        if (collectionsIncluded > 1)
        {
            query = query.AsSplitQuery();
        }

        if (options.Status is AccountStatus.Verified)
        {
            if (isEmailIdentity.HasValue)
            {
                query = isEmailIdentity.Value
                    ? query.Where(u => u.IsEmailVerified)
                    : query.Where(u => u.IsPhoneVerified);
            }
            else
            {
                query = query.Where(u => u.IsEmailVerified || u.IsPhoneVerified);
            }
        }
        else if (options.Status is AccountStatus.NotVerified)
        {
            if (isEmailIdentity.HasValue)
            {
                query = isEmailIdentity.Value
                    ? query.Where(u => !u.IsEmailVerified)
                    : query.Where(u => !u.IsPhoneVerified);
            }
            else
            {
                query = query.Where(u => !u.IsEmailVerified && !u.IsPhoneVerified);
            }
        }

        if (options.UserStatus.HasValue)
        {
            query = query.Where(u => u.Status == options.UserStatus.Value);
        }

        return query;
    }

    public async Task<bool> IsBannedAsync(string identity, CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsQueryable();

        bool isEmail = identity.Contains('@');

        if (isEmail)
        {
            var emailResult = Email.Create(identity);

            if (emailResult.IsError)
                return false;

            query = query.Where(u => u.PrimaryEmail == emailResult.Value);
        }
        else
        {
            query = query.Where(u => u.PhoneNumber == identity);
        }

        return await query.AnyAsync(u => u.Status == UserStatus.Banned, cancellationToken);
    }
}