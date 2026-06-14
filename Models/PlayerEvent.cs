namespace LevelUpLifeBackend.Models;

public class PlayerEvent
{
    public int Id { get; set; }
    public int PlayerUserId { get; set; }
    public PlayerUser? PlayerUser { get; set; }
    public PlayerEventType EventType { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
