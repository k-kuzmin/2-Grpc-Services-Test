using Domain;

namespace Infrastructure;

public class Repository<TEntity, TKey>(ApplicationDbContext context) : IRepository<TEntity, TKey>
    where TEntity : class, IEntity
{
    public IQueryable<TEntity> GetAll()
    {
        return context.Set<TEntity>();
    }

    public async Task<TEntity?> Get(TKey id, CancellationToken cancelationToken)
    {
        return await context.FindAsync<TEntity>(id, cancelationToken);
    }

    public async Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken)
    {
        await context.AddAsync(entity, cancelationToken);

        return entity;
    }

    public Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken)
    {
        context.Update(entity);

        return Task.FromResult(entity);
    }

    public async Task Delete(TKey id, CancellationToken cancelationToken)
    {
        var entity = await Get(id, cancelationToken);
        if (entity != null)
        {
            context.Remove(entity);
        }
    }
}
