namespace LevelUpLifeBackend.DTOs.Responses;

public class PurchaseRewardItemResponseDto
{
    public PlayerInventoryResponseDto Inventory { get; set; } = new();
    public int RemainingGold { get; set; }
}
