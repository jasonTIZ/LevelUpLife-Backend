namespace LevelUpLifeBackend.Models;

public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD,
    EPIC
}

public enum Frequency
{
    DAILY,
    WEEKLY
}

public enum PeriodUnit
{
    DAYS,
    WEEKS,
    MONTHS
}

public enum CompletionCriteria
{
    TIMER,
    REPETITIONS,
    EVIDENCE
}

public enum EvidenceType
{
    PHOTO,
    VIDEO,
    HEALTH_CONNECT
}
