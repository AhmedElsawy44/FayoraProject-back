using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.IdentityModule;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }
}
