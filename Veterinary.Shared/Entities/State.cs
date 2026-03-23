using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class State
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public Country Country { get; set; } = null!;

    public ICollection<City> Cities { get; set; } = new List<City>();
}
