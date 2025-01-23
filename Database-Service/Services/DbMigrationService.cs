using Context;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class DbMigrationService
    {
        private readonly ILogger<DbMigrationService> _logger;
        private readonly ApplicationDbContext _context;

        public DbMigrationService(
            ILogger<DbMigrationService> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Migrate()
        {
            try
            {
                _logger.LogInformation("Start migrate");
                _context.Database.Migrate();
            }
            catch (Exception ex) 
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }
    }
}
