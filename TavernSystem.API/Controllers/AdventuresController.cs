using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TavernSystem.Application;
using TavernSystem.Application.Interfaces;

namespace TavernSystem.API.Controllers;

[ApiController]
[Route("api/adventures")]
public class AdventuresController
{
    private readonly IAdventurerApplication _adventurerApplication;

    public AdventuresController(IAdventurerApplication adventurerApplication)
    {
        _adventurerApplication = adventurerApplication;
    }
    
    [HttpGet]
    public IResult GetAllAdventurers()
    {
        var adventurers = _adventurerApplication.GetAllAdventurers();
        return Results.Ok(adventurers);
    }

    [HttpGet("{id}")]
    public IResult GetSpecificAdventurer(int id)
    {
        var adventurer = _adventurerApplication.GetAdventurerById(id);
        return Results.Ok(adventurer);
    }

    [HttpPost]
    public IResult CreateAdventurer([FromBody]AdventurerDTO adventurer)
    {
        if (_adventurerApplication.CreateAdventurer(adventurer))
        {
            return Results.Ok();
        }
        else
        {
            return Results.BadRequest();
        }
    }
    
}

