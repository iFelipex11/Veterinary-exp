using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.DTOs;

public class ChangePasswordDTO
{
    [DataType(DataType.Password)]
    [Display(Name = "Contrasena actual")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string CurrentPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Nueva contrasena")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessage = "La nueva contrasena y la confirmacion no son iguales.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmacion nueva contrasena")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Confirm { get; set; } = null!;
}
