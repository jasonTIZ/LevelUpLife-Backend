namespace LevelUpLifeBackend.DTOs.Responses;

public class RewardItemResponseDto
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CostGold { get; set; }
    public decimal? EffectValue { get; set; }
    public bool IsActive { get; set; }
}
