using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Persistence.Repositories
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
