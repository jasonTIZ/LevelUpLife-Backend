namespace LevelUpLifeBackend.Models;

public class HabitDiscipline{
  public int Id { get; set; }
  public HabitCategory Category { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public bool IsActive { get; set; }

  public HabitDiscipline(){
    this.Id = 0;
    this.Category = new HabitCategory();
    this.Name = string.Empty;
    this.Description = string.Empty;
    this.IsActive = true;
  }
}
