using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MinLength(2, ErrorMessage = "Player name must be at least 2 characters long!")]
    public string Nickname { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }

    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
    
}