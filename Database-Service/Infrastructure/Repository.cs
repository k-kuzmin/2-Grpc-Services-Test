using Domain;

namespace Infrastructure;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity
{
    private readonly ApplicationDbContext _context;
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _context.Set<TEntity>();
    }

    public async Task<TEntity?> Get(TKey id, CancellationToken cancelationToken)
    {
        return await _context.FindAsync<TEntity>(id, cancelationToken);
    }

    public async Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken)
    {
        await _context.AddAsync(entity, cancelationToken);

        return entity;
    }

    public Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken)
    {
        _context.Update(entity);

        return Task.FromResult(entity);
    }

    public async Task Delete(TKey id, CancellationToken cancelationToken)
    {
        var entity = await Get(id, cancelationToken);
        if (entity != null)
        {
            _context.Remove(entity);
        }
    }
}
