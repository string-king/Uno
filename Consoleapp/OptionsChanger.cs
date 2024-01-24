using Domain;
using Helpers;

namespace Consoleapp;

public static class OptionsChanger
{
    public static string? ConfigureHandSize(GameOptions gameOptions)
    {
        var userChoice = ConsoleHelpers.ValidateIntegerInput("Enter initial hand size (2-10)", 2, 10);
        gameOptions.InitialHandSize = userChoice;
        Console.Clear();
        return null;
    }

    public static string? ConfigureForwardingCard(GameOptions gameOptions)
    {
        gameOptions.ForwardPlusCard = ConsoleHelpers.ValidateYesNoInput("Can forward +2 and +4 cards");
        Console.Clear();
        return null;
    }
}