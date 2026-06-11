using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class UpdateTimerCriteriaRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "NumSecondsDefined must be greater than 0.")]
    public int NumSecondsDefined { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "NumSecondsLong must be greater than 0.")]
    public int? NumSecondsLong { get; set; }

    [Required]
    public bool TypePauseIsAllowed { get; set; }

    public bool? IsActive { get; set; }
}
