using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public void AddUser(User user) => Add(user);

    public async Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken)
    {
        var emailObject = Email.Create(email).Value;

        return await context.Users.AnyAsync(u => u.PrimaryEmail == emailObject, cancellationToken);
    }

    public Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken) => IsExistAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

}
