using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fayora.Infrastructure.Persistence.Repositories;

public class BaseRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct
{
    protected readonly ApplicationDbContext context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }
    public void Add(TEntity entity) => DbSet.Add(entity);
    public void Remove(TEntity entity) => DbSet.Remove(entity);
    public Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) => DbSet.AnyAsync(predicate, cancellationToken);

    public Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken, bool isTracking = false) => (isTracking ? DbSet : DbSet.AsNoTracking()).FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool isTracking)
    {
        if (isTracking)
        {
            return DbSet.FindAsync(new[] { id }, cancellationToken).AsTask();
        }

        return DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }
}
