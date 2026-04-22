using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Los datos de la persona son requeridos.")]
    public PersonDataDto PersonData { get; set; } = new();

    [Required(ErrorMessage = "Los datos del usuario son requeridos.")]
    public PlayerUserDataDto PlayerUserData { get; set; } = new();
}

public class PersonDataDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es requerido.")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar 50 caracteres.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [StringLength(150, ErrorMessage = "El email no puede superar 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
    public DateOnly Birthdate { get; set; }
}

public class PlayerUserDataDto
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar 50 caracteres.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clase es requerida.")]
    public int ClassId { get; set; }
}
