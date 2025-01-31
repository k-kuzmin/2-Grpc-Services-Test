using System.ComponentModel.DataAnnotations;

namespace Domain;

public abstract class EntityBase
{
    [Key]
    public Guid Id { get; set; }
    public required DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }

    protected EntityBase() => Id = Guid.NewGuid();
}
