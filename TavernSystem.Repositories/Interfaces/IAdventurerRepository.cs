using TavernSystem.Application;

namespace TavernSystem.Repositories.Interfaces;

public interface IAdventurerRepository
{
    public IEnumerable<object> GetAllAdventurers();
    public ReturenAdventurerDTO? GetAdventurerById(int id);
    public bool CreateAdventurer(AdventurerDTO adventurer);
    public bool HasAnyBounties(string adventurerName);
    private bool IsValidPersonId(string personId)
    {
        throw new NotImplementedException();
    }
}