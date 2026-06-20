namespace LevelUpLifeBackend.DTOs.Responses;

public class ActivateInventoryItemResponseDto
{
    public PlayerInventoryResponseDto Inventory { get; set; } = new();
    public PlayerActiveEffectResponseDto? ActiveEffect { get; set; }
    public string? RecoveryMessage { get; set; }
}
