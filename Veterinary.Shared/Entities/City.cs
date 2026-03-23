using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class City
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    public int StateId { get; set; }

    public State State { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
