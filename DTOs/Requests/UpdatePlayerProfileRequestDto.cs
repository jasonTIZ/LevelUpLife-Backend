using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdatePlayerProfileRequestDto
{
    public PersonUpdateRequestDto? PersonData { get; set; }
    public PlayerDataUpdateRequestDto? PlayerData { get; set; }
}

public class PersonUpdateRequestDto
{
    [StringLength(80, MinimumLength = 2)]
    public string? Name { get; set; }

    [StringLength(80, MinimumLength = 2)]
    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateOnly? Birthdate { get; set; }
}

public class PlayerDataUpdateRequestDto
{
    [StringLength(20, MinimumLength = 3)]
    public string? UserName { get; set; }

    public int? PreferredClassId { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    // El modelo actual no persiste timezone, pero el contrato la acepta.
    public string? Timezone { get; set; }
}
