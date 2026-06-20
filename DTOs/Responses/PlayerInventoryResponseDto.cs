namespace LevelUpLifeBackend.DTOs.Responses;

public class PlayerInventoryResponseDto
{
    public int Id { get; set; }
    public int PlayerUserId { get; set; }
    public int RewardItemId { get; set; }
    public string RewardItemName { get; set; } = string.Empty;
    public int RewardItemTypeId { get; set; }
    public string RewardItemTypeName { get; set; } = string.Empty;
    public int CostGold { get; set; }
    public decimal? EffectValue { get; set; }
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime AcquiredAt { get; set; }
}
