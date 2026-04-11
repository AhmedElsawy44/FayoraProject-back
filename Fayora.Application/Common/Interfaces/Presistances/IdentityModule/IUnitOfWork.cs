using static Fayora.Application.Common.Interfaces.Services.AIModule.IAIService;

namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<object>> SearchAsync(string intent, SearchParams parameters);
}
