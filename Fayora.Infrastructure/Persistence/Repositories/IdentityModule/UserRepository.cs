using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

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


    public async Task<User?> GetUserByEmailAsync(string email, UserQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return null;

        var query = context.Users.Where(u => u.PrimaryEmail == emailResult.Value);

        options ??= new UserQueryOptions();
        query = ApplyQueryOptions(query, options, isEmailIdentity: true);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<User?> GetUserByPhoneAsync(string phoneNumber, UserQueryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

        var query = context.Users.Where(u => u.PhoneNumber == phoneNumber);

        options ??= new UserQueryOptions();
        query = ApplyQueryOptions(query, options, isEmailIdentity: false);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<User> ApplyQueryOptions(IQueryable<User> query, UserQueryOptions options, bool? isEmailIdentity = null)
    {

        if (options.IsReadOnly)
        {
            query = query.AsNoTracking();
        }

        int collectionsIncluded = 0;

        if (options.IncludeVerificationCodes)
        {
            query = query.Include(u => u.VerificationCodes);
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

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return false;

        return await context.Users
            .AnyAsync(u => u.PrimaryEmail == emailResult.Value, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }
}