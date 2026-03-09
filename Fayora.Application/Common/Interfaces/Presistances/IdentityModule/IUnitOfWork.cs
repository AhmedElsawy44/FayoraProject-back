namespace Fayora.Application.Common.Interfaces.Presistances.IdentityModule;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
