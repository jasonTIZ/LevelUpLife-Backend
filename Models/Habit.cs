namespace LevelUpLifeBackend.Models;

public class Habit
{
    public int Id { get; set; }
    public HabitDiscipline Discipline { get; set; }
    public PlayerUser User { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }

    public Habit()
    {
        this.Id = 0;
        this.Discipline = new HabitDiscipline();
        this.User = new PlayerUser();
        this.Title = string.Empty;
        this.Description = string.Empty;
        this.IsActive = true;
    }
}
