using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Currency
{
    [Key]
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public ICollection<User> Users { get; set; } = [];
}
