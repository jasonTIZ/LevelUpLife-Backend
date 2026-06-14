namespace LevelUpLifeBackend.Models;

public class StreakLog
{
    public int Id { get; set; }
    public int PlayerUserId { get; set; }
    public PlayerUser? PlayerUser { get; set; }
    public int StreakCount { get; set; }
    public DateOnly LogDate { get; set; }
    public bool ProtectionUsed { get; set; }
    public StreakProtectionType? ProtectionType { get; set; }
    public bool CompletionRecorded { get; set; }
}
