using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    // set the save location based on the user's profile folder
     private readonly string _saveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "uno_games");
     
    public void Save(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(_saveLocation))
        {
            Directory.CreateDirectory(_saveLocation);
        }

        File.WriteAllText(Path.Combine(_saveLocation, fileName), content);
    }

    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        if (!Directory.Exists(_saveLocation))
        {
            Directory.CreateDirectory(_saveLocation);
        }

        var data = Directory.EnumerateFiles(_saveLocation);

        // Filter out files based on the condition that the file name can be parsed as a GUID
        var gamefiles = data
            .Where(path =>
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path)?.Trim();
                return Guid.TryParse(fileNameWithoutExtension, out _);
            })
            .ToList();

        var res = gamefiles
            .Select(path => (
                id: Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                dt: File.GetLastWriteTime(path)
            ))
            .ToList();

        return res;
    }

    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");
        var filePath = Path.Combine(_saveLocation, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Game file not found: {fileName}");
        }

        var jsonStr = File.ReadAllText(Path.Combine(_saveLocation, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }
    
    public Dictionary<Game, GameState> GetGamesAndStates()
    {
        var dict = new Dictionary<Game, GameState>();
        
        foreach (var game in GetSaveGames())
        {
            var g = new Game()
            {
                Id = game.id,
                CreatedAtDt = game.dt,
                UpdatedAtDt = game.dt
            };
            dict[g] = LoadGame(g.Id);
        }

        return dict;
    }
}