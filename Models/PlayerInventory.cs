namespace LevelUpLifeBackend.Models;

public class PlayerInventory
{
    public int Id { get; set; }
    public int PlayerUserId { get; set; }
    public PlayerUser? PlayerUser { get; set; }
    public int RewardItemId { get; set; }
    public RewardItem? RewardItem { get; set; }
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime AcquiredAt { get; set; }
}
