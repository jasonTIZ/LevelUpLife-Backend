namespace LevelUpLifeBackend.Models;

public class Person{
  public int Id { get; set; }
  public string Name { get; set; }
  public string LastName { get; set; }
  public string PhoneNumber { get; set; }
  public string Email { get; set; }
  public  DateOnly BirthDate { get; set; }
  public bool IsActive { get; set; }

  public Person(){
    this.Id = 0;
    this.Name = string.Empty;
    this.LastName = string.Empty;
    this.PhoneNumber = string.Empty;
    this.Email = string.Empty;
    this.BirthDate = new DateOnly();
    this.IsActive = true;
  }
}
