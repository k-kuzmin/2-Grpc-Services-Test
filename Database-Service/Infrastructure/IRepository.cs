using Domain;

namespace Infrastructure;

public interface IRepository<TEntity, TKey> 
    where TEntity : class, IEntity
{
    IQueryable<TEntity> GetAll();
    Task<TEntity?> Get(TKey id, CancellationToken cancelationToken);
    Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken);
    Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken);
    Task Delete(TKey id, CancellationToken cancelationToken);
}
