using System.Text.Json.Serialization;

namespace LevelUpLifeBackend.DTOs.Requests;

public class RewardItemFilterRequestDto
{
    [JsonPropertyName("ID_REWARD_ITEM_TYPE")]
    public int? TypeId { get; set; }

    [JsonPropertyName("DSC_REWARD_ITEM_NAME")]
    public string? Name { get; set; }

    [JsonPropertyName("DSC_REWARD_ITEM_DESCRIPTION")]
    public string? Description { get; set; }

    [JsonPropertyName("NUM_REWARD_ITEM_COST_GOLD")]
    public int? CostGold { get; set; }

    [JsonPropertyName("NUM_REWARD_ITEM_EFFECT_VALUE")]
    public double? EffectValue { get; set; }
}
