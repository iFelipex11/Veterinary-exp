using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.Entities;

public class History
{
    public int Id { get; set; }

    [Display(Name = "Description")]
    [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    public string Description { get; set; } = null!;

    [Display(Name = "Date")]
    [Required(ErrorMessage = "The field {0} is mandatory.")]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    public string? Remarks { get; set; }

    [Display(Name = "Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime DateLocal => Date.ToLocalTime();

    public int ServiceTypeId { get; set; }

    public ServiceType ServiceType { get; set; } = null!;

    public int PetId { get; set; }

    public Pet Pet { get; set; } = null!;
}
