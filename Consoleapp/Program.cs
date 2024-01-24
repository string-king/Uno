// See https://aka.ms/new-console-template for more information

using Consoleapp;
using DAL;
using Domain;
using MenuSystem;
using UnoConsoleUI;
using UnoEngine;
using Helpers;
using Microsoft.EntityFrameworkCore;

Console.ForegroundColor = ConsoleColor.White;


// Custom game rules
var gameOptions = new GameOptions();


var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());
Console.WriteLine(connectionString);
var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using var db = new AppDbContext(contextOptions);
// apply all the migrations
db.Database.Migrate();
IGameRepository gameRepository = new GameRepositoryEF(db);

// state saving functionality, can be either file system based or db based. uses the same interface for both
// IGameRepository gameRepository = new GameRepositoryFileSystem();


var mainMenu = ProgramMenus.GetMainMenu(
    gameOptions,
    ProgramMenus.GetOptionsMenu(gameOptions),
    NewGame,
    LoadGame
);

// main menu
mainMenu.Run();

// exit program
return;


// start new game
string? NewGame()
{
    Console.Clear();
    // game logic, shared between console and web
    var gameEngine = new GameEngine(gameOptions);

    // set up players
    PlayerSetup.ConfigurePlayers(gameEngine);

    // set up the table
    gameEngine.InitializeDeckAndHands();

    // console controller for game loop
    var gameController = new GameController(gameEngine, gameRepository);

    gameController.Run();
    return null;
}

// load previously saved game
string? LoadGame()
{
    var savedGames = gameRepository.GetSaveGames();
    if (savedGames.Count == 0)
    {
        Console.WriteLine("\nNo saved games yet...");
        Console.WriteLine("\nPress enter to return to main menu");
        Console.ReadLine();
        Console.Clear();
        return "";
    }
    Console.WriteLine("Saved games");
    var saveGameListDisplay = savedGames.Select((s, i) => (i + 1) + " - " + s).ToList();

    Console.WriteLine(string.Join("\n", saveGameListDisplay));
    var userChoice = ConsoleHelpers.ValidateIntegerInput($"Select game to load (1-{saveGameListDisplay.Count})", 1,
        saveGameListDisplay.Count);
    var gameId = savedGames[userChoice - 1].id;
    var gameState = gameRepository.LoadGame(gameId);

    var gameEngine = new GameEngine
    {
        State = gameState
    };
    
    var gameController = new GameController(gameEngine, gameRepository);
    gameController.Run();

    return null;
}
