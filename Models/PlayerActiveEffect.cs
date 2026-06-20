namespace LevelUpLifeBackend.Models;

public class PlayerActiveEffect
{
    public int Id { get; set; }
    public int PlayerUserId { get; set; }
    public PlayerUser? PlayerUser { get; set; }
    public int InventoryId { get; set; }
    public PlayerInventory? Inventory { get; set; }
    public int RewardItemId { get; set; }
    public RewardItem? RewardItem { get; set; }
    public int RewardItemTypeId { get; set; }
    public decimal? EffectValue { get; set; }
    public int? RemainingCharges { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
