using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public class Currency : EntityBase
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public ICollection<User> Users { get; set; } = [];
}
