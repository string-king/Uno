
using System.ComponentModel.DataAnnotations;

namespace Domain;

public class GameOptions
{
    // hand size at game start
    [Range(2, 10, ErrorMessage = "The value must be between 2 and 10.")]
    public int InitialHandSize { get; set; } = 7;
    
    // allow plus card forwarding
    public bool ForwardPlusCard { get; set; } = false;

    public override string ToString() => $"\ninitial hand size: {InitialHandSize},\nforward draw cards: {(ForwardPlusCard ? "yes": "no")}";
}
