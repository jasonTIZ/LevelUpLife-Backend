namespace LevelUpLifeBackend.DTOs.Responses;

public class PlayerActiveEffectResponseDto
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    public int RewardItemId { get; set; }
    public string RewardItemName { get; set; } = string.Empty;
    public int RewardItemTypeId { get; set; }
    public string RewardItemTypeName { get; set; } = string.Empty;
    public decimal? EffectValue { get; set; }
    public int? RemainingCharges { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
