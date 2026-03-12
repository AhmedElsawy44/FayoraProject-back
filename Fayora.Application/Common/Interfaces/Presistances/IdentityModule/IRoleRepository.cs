using Fayora.Domain.Entitties.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IRoleRepository
{
    public Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
}
