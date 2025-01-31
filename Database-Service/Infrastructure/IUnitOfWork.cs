namespace Infrastructure;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
