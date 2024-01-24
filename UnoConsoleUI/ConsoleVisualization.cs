using Domain;

namespace UnoConsoleUI;

public static class ConsoleVisualization
{
    public static void DrawDesk(GameState state)
    {
        Console.WriteLine($"Card in play: {state.CardInPlay}");
        if (state.Forwarding)
        {
            Console.WriteLine("Forwarding allowed");
        }
        else
        {
            Console.WriteLine("Forwarding not allowed");
        }

        if (state.CardsToDraw > 0)
        {
            Console.WriteLine($"Cards to draw: {state.CardsToDraw}");
        }

        for (var i = 0; i < state.Players.Count; i++)
        {
            Console.WriteLine(
                $"Player {i + 1} - {state.Players[i].Nickname} has {state.Players[i].PlayerHand.Count} cards");
        }
    }

    public static void DrawPlayerHand(Player player)
    {
        Console.WriteLine("Your current hand is:");

        for (int i = 0; i < player.PlayerHand.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {player.PlayerHand[i]}");
        }
    }

}