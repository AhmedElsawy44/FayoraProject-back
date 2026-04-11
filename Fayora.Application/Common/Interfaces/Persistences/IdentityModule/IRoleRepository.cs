using Fayora.Domain.Entities.IdentityModule;

namespace Fayora.Application.Common.Interfaces.Persistences.IdentityModule;

public interface IRoleRepository
{
    public Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
}
