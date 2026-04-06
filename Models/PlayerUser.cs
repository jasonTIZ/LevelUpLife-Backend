namespace LevelUpLifeBackend.Models;

public class PlayerUser{
  public int Id { get; set; }
  public Person Person { get; set; }
  public UserPlayerClass Class { get; set; }
  public string UserName { get; set; }
  public string Password { get; set; }
  public int Level { get; set; }
  public int ExperiencePoints { get; set; }
  public int DaysStreak { get; set; }
  public DateTime LastLogin { get; set; }
  public DateTime CreationDate { get; set; }
  public bool IsActive { get; set; }

  public PlayerUser(){
    this.Id = 0;
    this.Person = new Person();
    this.Class = new UserPlayerClass();
    this.UserName = string.Empty;
    this.Password = string.Empty;
    this.Level = 0;
    this.ExperiencePoints = 0;
    this.DaysStreak = 0;
    this.LastLogin = new DateTime();
    this.CreationDate = new DateTime();
    this.IsActive = true;
  }
}
