namespace Services
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> Get(int id, CancellationToken cancelationToken);
        Task<TEntity> Create(TEntity entity, CancellationToken cancelationToken);
        Task<TEntity> Update(TEntity entity, CancellationToken cancelationToken);
        Task Delete(int id, CancellationToken cancelationToken);
        Task Save(CancellationToken cancelationToken);
    }
}
