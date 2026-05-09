using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateHabitRequestDto
{
    [Required(
        ErrorMessage = "Sin el identificador del habito no se puede realizar la actualización"
    )]
    [Range(1, int.MaxValue, ErrorMessage = "El habito que se quiere editar no es válido.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título del hábito es obligatorio.")]
    [StringLength(
        100,
        MinimumLength = 5,
        ErrorMessage = "El título debe de tener entre 5 y 100 caractares."
    )]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes de agregar una disciplina a este hábito.")]
    [Range(1, int.MaxValue, ErrorMessage = "La disciplina no es válida.")]
    public int DisciplineId { get; set; }

    [Required(ErrorMessage = "Él hábito debe de estar obligatoriamente asociado a un usuario.")]
    [Range(1, int.MaxValue, ErrorMessage = "Usuario inválido.")]
    public int UserId { get; set; }

    public List<UpdateHabitTaskRequestDto>? Tasks { get; set; }
}
