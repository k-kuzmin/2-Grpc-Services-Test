namespace Infrastructure;

public interface IUnitOfWork : IDisposable
{
    Task Commit(CancellationToken cancellationToken);
}
