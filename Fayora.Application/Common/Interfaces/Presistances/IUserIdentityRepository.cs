using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IUserIdentityRepository
{
    void AddIdentity(UserIdentity identity);
    public Task<UserIdentity?> GetIdentityByIdAsync(string Id, IdentityProvider identityProvider,CancellationToken cancellationToken = default);
}
