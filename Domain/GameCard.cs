namespace Domain;

public class GameCard
{
    public ECardColor Color { get; set; }
    public ECardValue Value { get; set; }

    public override string ToString()
    {
        return CardColorToString() + CardValueToString();
    }

    private string CardColorToString() =>
         Color switch
        {
            ECardColor.Red => "🔴",
            ECardColor.Green => "🟢",
            ECardColor.Blue => "🔵",
            ECardColor.Yellow => "🟡",
            ECardColor.Wild => "⚫️",
            _ => "NULLCOLOR"
        };

    private string CardValueToString() =>
        Value switch
        {
            ECardValue.Value0 => "0",
            ECardValue.Value1 => "1",
            ECardValue.Value2 => "2",
            ECardValue.Value3 => "3",
            ECardValue.Value4 => "4",
            ECardValue.Value5 => "5",
            ECardValue.Value6 => "6",
            ECardValue.Value7 => "7",
            ECardValue.Value8 => "8",
            ECardValue.Value9 => "9",
            ECardValue.ValueDraw2 => "+2",
            ECardValue.ValueDraw4 => "+4",
            ECardValue.ValueSkip => "🚫",
            ECardValue.ValueReverse => "🔄",
            ECardValue.ValueChangeColor => "🌈",
            _ => "NULLVALUE"
        };
    
    public string ToHtmlString()
    {
        string colorEmoji = CardColorToString();
        if (colorEmoji == "⚫️")
        {
            colorEmoji = "";
        }
        string valueString = CardValueToString();

        return $"<div class=\"card {Color.ToString().ToLower()} {Value.ToString().ToLower()}\">" +
               $"<div class=\"card-content\">" +
               $"<div class=\"card-value\">{colorEmoji} {valueString}</div>" +
               $"</div></div>";
    }
    
    public string ToHtmlStringUnplayable()
    {
        string colorEmoji = CardColorToString();
        if (colorEmoji == "⚫️")
        {
            colorEmoji = "";
        }
        string valueString = CardValueToString();

        return $"<div class=\"card unplayable {Color.ToString().ToLower()} {Value.ToString().ToLower()}\">" +
               $"<div class=\"card-content\">" +
               $"<div class=\"card-value\">{colorEmoji} {valueString}</div>" +
               $"</div></div>";
    }
}
