using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LevelUpLifeBackend.Models;

public class EvidenceStorage
{
    public int Id { get; set; }
    public int HabitTaskId { get; set; }
    public HabitTask? HabitTask { get; set; }
    public string? EvidencePathUrl { get; set; }
    public string? HealthDataJson { get; set; }
    public DateTime UploadedAt { get; set; }

    public EvidenceStorage()
    {
        UploadedAt = DateTime.UtcNow;
    }
}
