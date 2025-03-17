using Domain;

namespace Infrastructure;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
