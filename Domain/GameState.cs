namespace Domain;

public class GameState
{ 
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<GameCard> DeckOfCards { get; set; } = new List<GameCard>();
    public GameCard? FirstCard { get; set; }
    public CardInPlay CardInPlay { get; set; } = new CardInPlay();
    public int ActivePlayerNo { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public bool Reverse { get; set; }
    public int CardsToDraw { get; set; }
    public bool Forwarding { get; set; }
    public bool GameOver { get; set; }
    public Player? Winner { get; set; }
}