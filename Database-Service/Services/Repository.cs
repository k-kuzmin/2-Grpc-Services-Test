
using Context;

namespace Services
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
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

        public async Task<TEntity> Get(int id, CancellationToken cancelationToken)
        {
            return await _context.FindAsync<TEntity>(id, cancelationToken);
        }

        public async Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken)
        {
            await _context.AddAsync<TEntity>(entity, cancelationToken);

            return entity;
        }

        public Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken)
        {
            _context.Update(entity);

            return Task.FromResult(entity);
        }

        public async Task Delete(int id, CancellationToken cancelationToken)
        {
            var entity = await Get(id, cancelationToken);
            if (entity != null)
            {
                _context.Remove(entity);
            }
        }

        public async Task Save(CancellationToken cancelationToken)
        {
            await _context.SaveChangesAsync(cancelationToken);
        }
    }
}
