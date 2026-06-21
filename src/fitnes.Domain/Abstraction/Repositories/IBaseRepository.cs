namespace fitnes.Domain.Abstraction.Repositories;

public interface IBaseRepository<T, TKey> 
    where T : class, IEntity<TKey>
    where TKey : notnull
{
    Task<T> Create(T entity, CancellationToken cancellationToken);

    Task<T?> GetById(TKey id, CancellationToken cancellationToken);

    Task<bool> Update(T entity, CancellationToken cancellationToken);

    Task<bool> Delete(TKey id, CancellationToken cancellationToken);
}
