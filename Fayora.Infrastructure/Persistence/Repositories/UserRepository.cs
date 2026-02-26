using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.PrimaryEmail != null && u.PrimaryEmail.Value == email, cancellationToken);
    }

    public async Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }
}
