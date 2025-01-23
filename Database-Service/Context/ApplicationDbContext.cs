using Entities;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Favorites)
            .WithMany(c => c.Users);

        base.OnModelCreating(modelBuilder);
    }
}
