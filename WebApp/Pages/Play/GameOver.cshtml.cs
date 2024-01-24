using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Play;

public class GameOver : PageModel
{
    private readonly IGameRepository _repository;

    public GameEngine Engine { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    
    public GameOver(IGameRepository repository)
    {
        _repository = repository;
    }
    
    public void OnGet()
    {
        var gameState = _repository.LoadGame(GameId);
        Engine = new GameEngine
        {
            State = gameState
        };

        Engine.State.GameOver = true;
        Engine.State.Winner = Engine.GetActivePlayer();
        _repository.Save(GameId, Engine.State);
    }
}