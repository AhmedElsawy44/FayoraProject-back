using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule
{

    public class UserIdentityRepository(ApplicationDbContext context) : IUserIdentityRepository
    {
        public void AddIdentity(UserIdentity identity)
            => context.UserIdentities.Add(identity);

        public async Task<UserIdentity?> GetIdentityByIdAsync(
            string id,
            IdentityProvider identityProvider,
            CancellationToken cancellationToken = default)
        {
            return await context.UserIdentities
                .FirstOrDefaultAsync(
                    u => u.ProviderKey == id && u.Provider == identityProvider,
                    cancellationToken);
        }
    }

}
