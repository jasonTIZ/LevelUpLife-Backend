namespace LevelUpLifeBackend.Infrastructure.Configuration;

public class LevelingOptions
{
    public const string SectionName = "Leveling";

    public LevelingStrategy Strategy { get; set; } = LevelingStrategy.EscalatingPercent;

    public int BaseXpPerLevel { get; set; } = 100;

    public int EscalationPercent { get; set; } = 20;
}
