using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IUserIdentityRepository
{
    void AddIdentity(UserIdentity identity);
    public Task<UserIdentity?> GetIdentityByIdAsync(string Id, IdentityProvider identityProvider, CancellationToken cancellationToken = default);
}
