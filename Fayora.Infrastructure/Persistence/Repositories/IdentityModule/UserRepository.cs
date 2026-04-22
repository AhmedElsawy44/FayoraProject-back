// UserRepository.cs
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private static readonly UserQueryOptions DefaultOptions = new();
    private static readonly UserQueryOptions DefaultEmailOptions = new(IncludeVerificationCodes: false);

    public void AddUser(User user) => context.Users.Add(user);

    public async Task<User?> GetUserByIdAsync(
        Guid id,
        UserQueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (options is null)
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return await BuildBaseQuery(options)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(
        string email,
        UserQueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return null;

        options ??= DefaultEmailOptions;

        return await BuildBaseQuery(options)
        .FirstOrDefaultAsync(u => u.PrimaryEmail == emailResult.Value, cancellationToken);
    }

    public async Task<User?> GetUserByPhoneAsync(
        string phoneNumber,
        UserQueryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

        var phoneResult = PhoneNumber.Create(phoneNumber);
        if (phoneResult.IsError) return null;

        options ??= DefaultOptions;

        return await BuildBaseQuery(options)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneResult.Value, cancellationToken);
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
        var phoneResult = PhoneNumber.Create(phoneNumber);
        if (phoneResult.IsError) return false;

        return await context.Users
            .AnyAsync(u => u.PhoneNumber != null && u.PhoneNumber == phoneResult.Value, cancellationToken);
    }


    private IQueryable<User> BuildBaseQuery(UserQueryOptions options)
    {
        var query = options.IsReadOnly
            ? context.Users.AsNoTracking()
            : context.Users.AsQueryable();

        if (options.IncludeVerificationCodes)
            query = query.Include(u => u.VerificationCodes);

        query = options.Status switch
        {
            AccountStatus.Verified => query.Where(u => u.IsEmailVerified || u.IsPhoneVerified),
            AccountStatus.NotVerified => query.Where(u => !u.IsEmailVerified && !u.IsPhoneVerified),
            _ => query  
        };

        if (options.ShouldFilterByUserStatus)
            query = query.Where(u => (u.Status & options.UserStatus) != 0);

        return query;
    }
}