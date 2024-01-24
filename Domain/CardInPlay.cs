namespace Domain;

public class CardInPlay : GameCard
{
    private GameCard? Card { get; set; } = new GameCard();
    
    public void Set(GameCard card)
    {
        Color = card.Color;
        Value = card.Value;
        Card = card;
    }

    public override string ToString()
    {
        return Card!.ToString();
    }

    public void SetColor(ECardColor newColor)
    {
        Color = newColor;
        Card = new GameCard()
        {
            Color = Color,
            Value = Card!.Value
        };
    }
}