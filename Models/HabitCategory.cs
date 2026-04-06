namespace LevelUpLifeBackend.Models;

public class HabitCategory{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public bool IsActive { get; set; }

  public HabitCategory(){
    this.Id = 0;
    this.Name = string.Empty;
    this.Description = string.Empty;
    this.IsActive = true;
  }
}
