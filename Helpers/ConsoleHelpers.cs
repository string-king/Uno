namespace Helpers;

public class ConsoleHelpers
{
    public static int ValidateIntegerInput(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int value) && value >= minValue && value <= maxValue)
            {
                return value;
            }
            Console.WriteLine($"Invalid input. Please enter a number between {minValue} and {maxValue}.");
        }
    }

    public static string ValidatePlayerType(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var playerType = Console.ReadLine()?.ToLower().Trim();
            if (playerType == "a" || playerType == "h")
            {
                return playerType;
            }
            Console.WriteLine("Invalid input. Please enter 'A' for AI or 'H' for Human.");
        }
    }

    public static string ValidatePlayerName(string prompt, string defaultValue)
    {
        Console.Write($"{prompt}, press enter for {defaultValue}: ");
        while (true)
        {
            var playerName = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = defaultValue;
            }

            if (!string.IsNullOrWhiteSpace(playerName) && playerName.Length > 0)
            {
                return playerName;
            }
            Console.WriteLine("Invalid input. Please enter a valid player name.");
        }
    }

    public static bool ValidateYesNoInput(string prompt)
    {
        Console.Write($"{prompt} (Y/N): ");
        while (true)
        {
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "y" || input == "n")
            {
                return input == "y";
            }
            Console.WriteLine("Invalid input. Please enter 'Y' or 'N'.");
        }
    }
    
    public static string ValidateOptionFromListOfStrings(string prompt, List<String> options)
    {
        var optionsStr = string.Join(", ", options);
        Console.Write($"{prompt}: ");
        while (true)
        {
            var choice = Console.ReadLine()?.ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(choice) && options.Contains(choice))
            {
                return choice;
            }
            Console.WriteLine($"Invalid input. Please enter of the following: {optionsStr}");
        }
    }
}