using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IUserIdentityRepository
{
    void AddIdentity(UserIdentity identity);
    public Task<UserIdentity?> GetIdentityByIdAsync(string Id, IdentityProvider identityProvider, CancellationToken cancellationToken = default);
}
