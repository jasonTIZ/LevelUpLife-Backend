namespace LevelUpLifeBackend.Models;

public class UserPlayerClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double XpMultStudy { get; set; }
    public double XpMultSport { get; set; }
    public double XpMultFood { get; set; }
    public bool IsActive { get; set; }

    public UserPlayerClass()
    {
        this.Id = 0;
        this.Name = string.Empty;
        this.Description = string.Empty;
        this.XpMultStudy = 0;
        this.XpMultSport = 0;
        this.XpMultFood = 0;
        this.IsActive = true;
    }
}
