using Fayora.Domain.Entities.Identity;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUserRepository
{
    Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken);
    Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken);
    void AddUser(User user);
}
