using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.DTOs;

public class EmailDTO
{
    [Display(Name = "Correo")]
    [EmailAddress(ErrorMessage = "Debes ingresar un correo valido.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Email { get; set; } = null!;
}
