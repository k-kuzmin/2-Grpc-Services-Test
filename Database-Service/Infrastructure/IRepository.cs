using Domain;

namespace Infrastructure;

public interface IRepository<TEntity> where TEntity : EntityBase
{
    IQueryable<TEntity> GetAll();
    Task<TEntity?> Get(Guid id, CancellationToken cancelationToken);
    Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken);
    Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken);
    Task Delete(Guid id, CancellationToken cancelationToken);
}
