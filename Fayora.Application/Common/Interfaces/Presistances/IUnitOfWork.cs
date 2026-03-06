namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IUnitOfWork
{
    Task CommitChangesAsync(CancellationToken cancellationToken);
}
