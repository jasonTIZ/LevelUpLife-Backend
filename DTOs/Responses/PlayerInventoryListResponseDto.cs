namespace LevelUpLifeBackend.DTOs.Responses;

public class PlayerInventoryListResponseDto
{
    public IEnumerable<PlayerInventoryResponseDto> Items { get; set; } = [];
    public IEnumerable<PlayerActiveEffectResponseDto> ActiveEffects { get; set; } = [];
}
