namespace TavernSystem.Application;

public class ReturenAdventurerDTO
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Race { get; set; }
    public string Experience { get; set; }
    public Person PersonData { get; set; }
}