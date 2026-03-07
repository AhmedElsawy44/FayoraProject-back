namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
