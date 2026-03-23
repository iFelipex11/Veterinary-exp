using System.ComponentModel.DataAnnotations;

namespace Veterinary.Shared.DTOs;

public class ResetPasswordDTO
{
    [Display(Name = "Correo")]
    [EmailAddress(ErrorMessage = "Debes ingresar un correo valido.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El token es obligatorio.")]
    public string Token { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Contrasena")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "La contrasena y la confirmacion no son iguales.")]
    [Display(Name = "Confirmacion de contrasena")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.")]
    public string ConfirmPassword { get; set; } = null!;
}
