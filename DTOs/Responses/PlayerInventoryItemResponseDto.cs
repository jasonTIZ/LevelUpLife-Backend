namespace LevelUpLifeBackend.DTOs.Responses;

public class PlayerInventoryItemResponseDto
{
    public int Id { get; set; }
    public int RewardItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemDescription { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime AcquiredAt { get; set; }
    public int CostGold { get; set; }
    public decimal? EffectValue { get; set; }
}
