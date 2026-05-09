using System.ComponentModel.DataAnnotations;
using LevelUpLifeBackend.Validators;

namespace LevelUpLifeBackend.DTOs.Requests;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El usuario o email es requerido.")]
    [UserNameOrEmail]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Debe tener entre 6 y 100 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
