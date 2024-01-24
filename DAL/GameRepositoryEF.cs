using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    private readonly AppDbContext _ctx;

    public GameRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public void Save(Guid id, GameState state)
    {
        var game = _ctx.Games.FirstOrDefault(g => g.Id == state.Id);
        if (game == null)
        {
            game = new Game()
            {
                Id = state.Id,
                State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
            };
            _ctx.Games.Add(game);
        }
        else
        {
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        }

        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }

    public GameState LoadGame(Guid id)
    {
        var game = _ctx.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<GameState>(game.State, JsonHelpers.JsonSerializerOptions)!;
    }

    public Dictionary<Game, GameState> GetGamesAndStates()
    {
        var dict = new Dictionary<Game, GameState>();
        foreach (var game in _ctx.Games)
        {
            dict[game] = LoadGame(game.Id);
        }

        return dict;
    }
}