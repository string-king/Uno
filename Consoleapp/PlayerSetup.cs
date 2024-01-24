using Domain;
using UnoEngine;
using Helpers;

namespace Consoleapp;

public static class PlayerSetup
{
    public static void ConfigurePlayers(GameEngine gameEngine)
    {
        
        var playerCount = ConsoleHelpers.ValidateIntegerInput("How many players (2 - 10)", 2, 10);

        for (int i = 0; i < playerCount; i++)
        {
            string? playerType = ConsoleHelpers.ValidatePlayerType($"Choose player {i + 1} type (A - ai or H - human)");

            string defaultName;
            if (playerType == "h")
            {
                 defaultName = $"Human {(i + 1)}";
            }
            else
            {
                defaultName = $"Ai {(i + 1)}";
            }
            var playerName = ConsoleHelpers.ValidatePlayerName($"Player {i + 1} name (min 1 letter)", defaultName);

            gameEngine.State.Players.Add(new Player()
            {
                Nickname = playerName,
                PlayerType = playerType == "h" ? EPlayerType.Human : EPlayerType.AI
            });
        }
    }
}
