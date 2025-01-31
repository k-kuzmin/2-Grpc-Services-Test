using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntitiesConfiguration;

public class UserConfiiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(x => x.Favorites)
            .WithMany(x => x.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserCurrency",
                role => role
                    .HasOne<Currency>()
                    .WithMany()
                    .HasForeignKey("CurrencyId")
                    .OnDelete(DeleteBehavior.Cascade),
                user => user
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade));
    }
}
