using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class Agenda
{
    public int Id { get; set; }

    [Display(Name = "Date")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd h:mm tt}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    public int OwnerId { get; set; }

    public Owner Owner { get; set; } = null!;

    public int PetId { get; set; }

    public Pet Pet { get; set; } = null!;

    public string? Remarks { get; set; }

    [Display(Name = "Is Available?")]
    public bool IsAvailable { get; set; }
}
