
using Domain;

namespace UnoEngine;

public class GameEngine
{
    public GameState State { get; set; } = new GameState();
    private readonly Random _rnd = new Random();
    private readonly GameOptions? _gameOptions;
    
    public GameEngine(GameOptions gameOptions)
    {
        _gameOptions = gameOptions;
        State.Forwarding = _gameOptions.ForwardPlusCard;
    }
    
    public GameEngine()
    {
    }

    public void InitializeDeckAndHands()
    {
        CreateNewRandomizedDeck();
        DealPlayerHandsAndSetFirstCard();
    }

    private void CreateNewRandomizedDeck()
    {
        var deck = new List<GameCard>();

        // Add colored cards to the deck
        foreach (ECardColor color in Enum.GetValues(typeof(ECardColor)))
        {
            if (color == ECardColor.Wild)
                continue;

            foreach (ECardValue value in Enum.GetValues(typeof(ECardValue)))
            {
                if (value == ECardValue.ValueChangeColor || value == ECardValue.ValueDraw4)
                    continue; 

                int cardCount = (value == ECardValue.Value0) ? 1 : 2;
                for (int i = 0; i < cardCount; i++)
                {
                    deck.Add(new GameCard { Color = color, Value = value });
                }
            }
        }

        // Add wild cards to the deck
        foreach (ECardValue value in Enum.GetValues(typeof(ECardValue)))
        {
            if (value >= ECardValue.ValueDraw4 && value <= ECardValue.ValueChangeColor)
            {
                for (int i = 0; i < 4; i++)
                {
                    deck.Add(new GameCard { Color = ECardColor.Wild, Value = value });
                }
            }
        }

        // Shuffle the deck
        deck = ShuffleDeck(deck);

        // Set the shuffled deck as the game deck
        State.DeckOfCards = deck;
    }

    private List<GameCard> ShuffleDeck(List<GameCard> deck)
    {
        List<GameCard> shuffledDeck = new List<GameCard>();
        Random rnd = new Random();

        while (deck.Count > 0)
        {
            int randomPosInDeck = rnd.Next(deck.Count);
            shuffledDeck.Add(deck[randomPosInDeck]);
            deck.RemoveAt(randomPosInDeck);
        }

        return shuffledDeck;
    }

    private void DealPlayerHandsAndSetFirstCard()
    {
        CreateNewRandomizedDeck();

        foreach (var player in State.Players)
        {
            var cards = GetCard(_gameOptions!.InitialHandSize);
            player.PlayerHand = cards;
            //SortPlayerHand(player);
        }
        
        // deal the first card on the table
        
        var randomDeckIndex = _rnd.Next(State.DeckOfCards.Count);
        var randomCard = State.DeckOfCards[randomDeckIndex];
        
        // first card cannot be wild or + card
        
        while (randomCard.Color == ECardColor.Wild || randomCard.Value == ECardValue.ValueDraw2
                                                   || randomCard.Value == ECardValue.ValueDraw4
                                                   || randomCard.Value == ECardValue.ValueReverse
                                                   || randomCard.Value == ECardValue.ValueSkip)
        {
            randomDeckIndex = _rnd.Next(State.DeckOfCards.Count);
            randomCard = State.DeckOfCards[randomDeckIndex];
        }

        State.FirstCard = randomCard;
        State.CardInPlay.Set(randomCard);
        State.DeckOfCards.RemoveAt(randomDeckIndex);
    }

    public List<GameCard> GetCard(int amount)
    {
        var cards = new List<GameCard>();
        var i= amount;
        while (i > 0)
        {
            if (State.DeckOfCards.Count == 0)
            {
                CreateNewRandomizedDeck();
            }
            var card = State.DeckOfCards[0];
            cards.Add(card);
            State.DeckOfCards.RemoveAt(0);
            i--;
        }
        return cards;
    }
    
    public void PlayCard(GameCard card)
    {
        State.CardInPlay.Set(card);
        GetActivePlayerHand().Remove(card);
        
        switch (card.Value)
        {
            case ECardValue.ValueReverse:
                State.Reverse = !State.Reverse;
                break;
            case ECardValue.ValueSkip:
                HandleSkipCard();
                break;
            case ECardValue.ValueDraw2:
                State.CardsToDraw += 2;
                break;
            case ECardValue.ValueDraw4:
                State.CardsToDraw += 4;
                break;
        }
    }

    public void AiPlayCard()
    {
        if (GetActivePlayer().PlayerType != EPlayerType.AI)
        {
            throw new Exception("Cant play card for AI, active player is not AI!");
        }

        if (!HandHasPlayableCard(GetActivePlayer().PlayerHand))
        {
            throw new Exception("Cant play card for AI, hand has no playable cards!");
        }

        var playableCards = GetActivePlayerHand().Where(IsCardPlayable);
        GameCard cardToPlay;

        if (GetNextPlayer().PlayerHand.Count < 4)
        {
            if (playableCards.Where(IsDrawCard).Any())
            {
                cardToPlay = playableCards.Where(IsDrawCard).First();
            }
            else
            {
                cardToPlay = playableCards.First();
            }
        }
        else
        {
            cardToPlay = playableCards.First();
        }
        
        PlayCard(cardToPlay);

        if (cardToPlay.Color == ECardColor.Wild)
        {
            var colorIndex = _rnd.Next(0, 4);
            var colors = Enum.GetValues<ECardColor>();
            ECardColor newColor = colors[colorIndex];
            State.CardInPlay.SetColor(newColor);
        }
        
    }

    private Player GetNextPlayer()
    {
        Player nextPlayer;
        
        if (State.Reverse)
        {
            nextPlayer = (State.ActivePlayerNo == 0) ? State.Players[State.Players.Count() - 1] : State.Players[State.ActivePlayerNo - 1];
        }
        else
        {
            nextPlayer = (State.ActivePlayerNo == State.Players.Count - 1) ? State.Players[0] : State.Players [State.ActivePlayerNo + 1];
        }

        return nextPlayer;
    }

    private void HandleSkipCard()
    {
        var playerCount = State.Players.Count;

        if (State.Reverse)
        {
            State.ActivePlayerNo = (State.ActivePlayerNo == 0) ? playerCount - 1 : State.ActivePlayerNo - 1;
        }
        else
        {
            State.ActivePlayerNo = (State.ActivePlayerNo == playerCount - 1) ? 0 : State.ActivePlayerNo + 1;
        }
    }

    public void SetNewCardInPlayColor(ECardColor newColor)
    {
        State.CardInPlay.SetColor(newColor);
    }

    public void NextPlayerTurn()
    {
        SortPlayerHand(GetActivePlayer());
        
        if (State.Reverse)
        {
            State.ActivePlayerNo--;
            if (State.ActivePlayerNo < 0) State.ActivePlayerNo = State.Players.Count - 1;
        }
        else
        {
            State.ActivePlayerNo++;
            if (State.ActivePlayerNo >= State.Players.Count) State.ActivePlayerNo = 0;
        }
    }

    public void SortPlayerHand(Player player)
    {
        player.PlayerHand = player.PlayerHand
            .OrderBy(c => c.Color)
            .ThenBy(c => c.Value)
            .ToList();
    }


    public bool IsCardPlayable(GameCard playerCard)
    {
        var cardInPlay = State.CardInPlay;

        if (State.Forwarding && State.CardsToDraw > 0)
        {
            return IsDrawCard(playerCard) && (playerCard.Color == cardInPlay.Color || playerCard.Value == ECardValue.ValueDraw4);
        }

        if (!IsColorCard(playerCard))
        {
            return true;
        }
        
        if (cardInPlay.Color == playerCard.Color)
        {
            return true;
        }
        
        return cardInPlay.Value == playerCard.Value;
    }

    public bool HandHasPlayableCard(List<GameCard> hand)
    {
        return hand.Any(IsCardPlayable);
    }

    private static bool IsDrawCard(GameCard card)
    {
        return card.Value is ECardValue.ValueDraw2 or ECardValue.ValueDraw4;
    }

    public bool IsColorCard(GameCard card)
    {
        return card.Color is not ECardColor.Wild;
    }
    

    public bool ActivePlayerHasOneCard()
    {
        return GetActivePlayerHand().Count == 1;
    }

    public Player GetActivePlayer()
    {
        return State.Players[State.ActivePlayerNo];
    }

    public List<GameCard> GetActivePlayerHand()
    {
        return GetActivePlayer().PlayerHand;
    }
}
