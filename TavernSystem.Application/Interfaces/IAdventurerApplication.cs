namespace TavernSystem.Application.Interfaces;

public interface IAdventurerApplication
{
    public IEnumerable<object> GetAllAdventurers();
    public bool CreateAdventurer(AdventurerDTO adventurer);
    public ReturenAdventurerDTO? GetAdventurerById(int id);
}