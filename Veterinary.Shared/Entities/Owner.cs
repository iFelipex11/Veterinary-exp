using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class Owner
{
    public int Id { get; set; }

    [Display(Name = "Document")]
    [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    public string Document { get; set; } = null!;

    [Display(Name = "First Name")]
    [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name")]
    [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Fixed Phone")]
    [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    public string? FixedPhone { get; set; }

    [Display(Name = "Address")]
    [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    public string? Address { get; set; }

    [Display(Name = "Image")]
    public string? Photo { get; set; }

    [Display(Name = "Remarks")]
    public string? Remarks { get; set; }

    [Display(Name = "Cell Phone")]
    [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    public string? CellPhone { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();
}
