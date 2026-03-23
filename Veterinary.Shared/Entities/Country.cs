using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class Country
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    public ICollection<State> States { get; set; } = new List<State>();
}
