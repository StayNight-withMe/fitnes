using fitnes.Domain.Abstraction.Repositories;
using fitnes.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace fitnes.Infrastructure.Persistence.Repositories.Base;

public class PgBaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class, IEntity<TKey> where TKey : notnull
{
    private ApplicationDbContext _dbContext;
    private DbSet<T> _entitySet;

    public PgBaseRepository(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
        _entitySet = _dbContext.Set<T>();
    }

    public virtual async Task<T> Create(T entity, CancellationToken cancellationToken)
    {
        await _entitySet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<bool> Delete(TKey id, CancellationToken cancellationToken)
    {
        var entity = await _entitySet.FindAsync(id, cancellationToken);

        if (entity is not null)
        {
            _entitySet.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public virtual async Task<T?> GetById(TKey id, CancellationToken cancellationToken)
    {
        var entity = await _entitySet.AsNoTracking().FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
        return entity;
    }

    public virtual async Task<bool> Update(T entity, CancellationToken cancellationToken)
    {
        _entitySet.Update(entity);
        var count = await _dbContext.SaveChangesAsync();
        if(count != default)
        {
            return true;
        }
        return false;
    }
}
