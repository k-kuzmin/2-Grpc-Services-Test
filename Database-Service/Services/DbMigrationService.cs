using Domain;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class DbMigrationService(
    ILogger<DbMigrationService> logger,
    ApplicationDbContext context)
{
    public void Migrate()
    {
        try
        {
            logger.LogInformation("Start migrate");
            context.Database.Migrate();
        }
        catch (Exception ex) 
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}
