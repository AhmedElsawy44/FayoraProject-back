namespace Fayora.Application.Common.Interfaces.Persistences.IdentityModule;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
