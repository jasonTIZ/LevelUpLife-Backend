namespace LevelUpLifeBackend.Models;

public class RewardItem
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public RewardItemType? Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CostGold { get; set; }
    public decimal? EffectValue { get; set; }
    public int? DurationDays { get; set; }
    public bool IsActive { get; set; }
}
