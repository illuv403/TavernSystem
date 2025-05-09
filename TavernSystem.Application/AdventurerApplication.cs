using TavernSystem.Application.Interfaces;
using TavernSystem.Repositories;
using TavernSystem.Repositories.Interfaces;

namespace TavernSystem.Application;

public class AdventurerApplication : IAdventurerApplication
{
    private readonly IAdventurerRepository _adventurerRepository;

    public AdventurerApplication(string connectionString)
    {
        _adventurerRepository = new AdventurerRepository(connectionString);
    }
    
    public IEnumerable<object> GetAllAdventurers() => _adventurerRepository.GetAllAdventurers();

    public bool CreateAdventurer(AdventurerDTO adventurer)
    {
        if (_adventurerRepository.CreateAdventurer(adventurer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ReturenAdventurerDTO? GetAdventurerById(int id) => _adventurerRepository.GetAdventurerById(id);
    
}