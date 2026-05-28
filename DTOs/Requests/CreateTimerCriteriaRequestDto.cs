using System.ComponentModel.DataAnnotations;

namespace LevelUpLifeBackend.DTOs.Requests;

public class CreateTimerCriteriaRequestDto
{
    [Required(ErrorMessage = "NUM_SECONDS_DEFINED is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "NUM_SECONDS_DEFINED must be greater than 0.")]
    public int? NUM_SECONDS_DEFINED { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "NUM_SECONDS_LONG must be greater than 0.")]
    public int? NUM_SECONDS_LONG { get; set; }

    [Required(ErrorMessage = "TYPE_PAUSE_IS_ALLOWED is required.")]
    public bool? TYPE_PAUSE_IS_ALLOWED { get; set; }

    public bool? STATUS_TIMER_CRITERIA_IS_ACTIVE { get; set; }
}
