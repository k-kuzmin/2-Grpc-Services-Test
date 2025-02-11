namespace Domain.Entities;

public class Currency : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public ICollection<User> Users { get; set; } = [];
}
