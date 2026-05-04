namespace LevelUpLifeBackend.Models;

public class RepetitionCriteria
{
    public int Id { get; set; }
    public int HabitTaskId { get; set; }
    public HabitTask? HabitTask { get; set; }
    public int NumRepetitionsObjective { get; set; }
    public MeasurementUnit TypeUnityMeasurementUnit { get; set; }
    public bool StatusIsPartialAllowed { get; set; }
    public bool StatusRepetitionsCriteriaIsActive { get; set; }
public RepetitionCriteria()
{
    Id = 0;
    HabitTaskId = 0;
    NumRepetitionsObjective = 0;
    TypeUnityMeasurementUnit = MeasurementUnit.REPS;
    StatusIsPartialAllowed = false;
    StatusRepetitionsCriteriaIsActive = true;
}
}