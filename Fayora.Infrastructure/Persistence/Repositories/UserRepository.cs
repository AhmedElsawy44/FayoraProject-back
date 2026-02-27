using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Domain.Entities.Identity;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public void AddUser(User user) => Add(user);

    public async Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken) => await IsExistAsync(u => u.PrimaryEmail != null && u.PrimaryEmail.Value == email, cancellationToken);

    public async Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken) => await IsExistAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
}
