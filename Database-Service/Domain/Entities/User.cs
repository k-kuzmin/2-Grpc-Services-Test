using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser, IEntity<string>
{
    public ICollection<Currency> Favorites { get; set; } = [];
}
