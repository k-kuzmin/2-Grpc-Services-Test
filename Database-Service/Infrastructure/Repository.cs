using Domain;

namespace Infrastructure;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : EntityBase
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

    public async Task<TEntity?> Get(Guid id, CancellationToken cancelationToken)
    {
        return await _context.FindAsync<TEntity>(id, cancelationToken);
    }

    public async Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken)
    {
        entity.DateCreated = DateTimeOffset.UtcNow;
        await _context.AddAsync(entity, cancelationToken);

        return entity;
    }

    public Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken)
    {
        entity.DateUpdated = DateTimeOffset.UtcNow;
        _context.Update(entity);

        return Task.FromResult(entity);
    }

    public async Task Delete(Guid id, CancellationToken cancelationToken)
    {
        var entity = await Get(id, cancelationToken);
        if (entity != null)
        {
            _context.Remove(entity);
        }
    }
}
