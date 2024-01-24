using DAL;
using Domain;
using UnoEngine;
using static Helpers.ConsoleHelpers;

namespace UnoConsoleUI;

public class GameController
{
    private readonly GameEngine _engine;
    private readonly IGameRepository _repository;
    
    public GameController(GameEngine engine, IGameRepository repository)
    {
        _engine = engine;
        _repository = repository;
    }

    public void Run()
    {
        Console.Clear();
        while (true)
        {
            // check if loaded game is over   
            if (_engine.State.GameOver)
            {
                HandleGameOver();
                Console.Clear();
                return;
            }

            if (_engine.GetActivePlayer().PlayerType == EPlayerType.AI)
            {
                HandleAiTurn();

                if (_engine.State.GameOver)
                {
                    Console.ReadKey();
                    return;
                }
                
                var cont = EndOfTurn();
                if (cont == false)
                {
                    break;
                }
                Console.Clear();
                continue;
            }
            
            // one move in loop
            DisplayTurnStartMessage();
            Console.Clear();
            Console.WriteLine($"Player {_engine.State.ActivePlayerNo + 1} - {_engine.GetActivePlayer().Nickname}");
            ConsoleVisualization.DrawDesk(_engine.State);
            
            bool? continueGame = null;
            
            if (_engine.State.CardsToDraw > 0)
            {
                if (_engine.State.Forwarding == false || !_engine.HandHasPlayableCard(_engine.GetActivePlayer().PlayerHand))
                {
                    HandleCardsToPickUp();
                    continueGame = EndOfTurn();
                }
            }
            else if (!_engine.HandHasPlayableCard(_engine.GetActivePlayer().PlayerHand))
            {
                continueGame = HandleNoPlayableCards();
            }

            if (continueGame != null)
            {
                if (continueGame == true)
                {
                    continue;
                }
                break;
            }
            
            // player playing a card
            ConsoleVisualization.DrawPlayerHand(_engine.GetActivePlayer());
            while (true)
            {
                var playerChoice =
                    ValidateIntegerInput(
                        $"Choose card to play (number 1 to {_engine.GetActivePlayer().PlayerHand.Count})", 1,
                        _engine.GetActivePlayer().PlayerHand.Count);
                var selectedCard = _engine.GetActivePlayer().PlayerHand[playerChoice - 1];
                
                if (_engine.IsCardPlayable(selectedCard) == false)
                {
                    Console.WriteLine("Selected card is not legal");
                    continue;
                }
                
                _engine.PlayCard(selectedCard);
                if (!_engine.IsColorCard(selectedCard))
                {
                    _engine.SetNewCardInPlayColor(SelectNewCardColor());
                }
                
                if (_engine.GetActivePlayer().PlayerHand.Count == 0)
                {
                    HandleGameOver();
                    break;
                }
                HandleUno();
                break;
            }
            if (_engine.State.GameOver)
            {
                break;
            }
            Console.WriteLine();
            ConsoleVisualization.DrawDesk(_engine.State);
            continueGame = EndOfTurn();
            if (continueGame == false) break;
            Console.Clear();
        }
    }

    private void HandleAiTurn()
    {
        Console.Clear();
        ConsoleVisualization.DrawDesk(_engine.State);
        Console.WriteLine($"\nPlayer {_engine.State.ActivePlayerNo + 1} - {_engine.GetActivePlayer().Nickname} turn:");

        if (_engine.State.CardsToDraw > 0)
        {
            if (_engine.State.Forwarding && _engine.HandHasPlayableCard(_engine.GetActivePlayerHand()))
            {
                _engine.AiPlayCard();
                Console.WriteLine("Played " + _engine.State.CardInPlay);
            }
            else
            {
                _engine.GetActivePlayer().PlayerHand.AddRange(_engine.GetCard(_engine.State.CardsToDraw));
                Console.WriteLine($"Picked up {_engine.State.CardsToDraw} cards");
                _engine.State.CardsToDraw = 0;                
            }
        }
        
        else if (!_engine.HandHasPlayableCard(_engine.GetActivePlayerHand()))
        {
            Console.WriteLine("No playable cards, picked up 1 card.");
            _engine.GetActivePlayerHand().Add(_engine.GetCard(1).First());
        }

        else
        {
            _engine.AiPlayCard();
            Console.WriteLine("Played " + _engine.State.CardInPlay);
        }

        if (_engine.GetActivePlayerHand().Count == 1)
        {
            Console.WriteLine("UNO!");
        }
        
        if (_engine.GetActivePlayerHand().Count == 0)
        {
           HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        if (_engine.GetActivePlayerHand().Count == 0)
        {
            _engine.State.GameOver = true;
            _engine.State.Winner = _engine.GetActivePlayer();
            _repository.Save(_engine.State.Id, _engine.State);
        
            Console.WriteLine("Game over.");
            Console.WriteLine($"Congratulations to the winner Player {_engine.State.ActivePlayerNo + 1}:" +
                              $" {_engine.GetActivePlayer().Nickname}!");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Console.Clear();
        }
        
    }
    
    private void DisplayTurnStartMessage()
    {
        Console.WriteLine($"Player {_engine.State.ActivePlayerNo + 1} - {_engine.GetActivePlayer().Nickname}");
        Console.WriteLine("Your turn, make sure you are looking at the screen alone!");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void HandleCardsToPickUp()
    {
        Console.WriteLine($"Picked up {_engine.State.CardsToDraw} cards:");
        var cards = _engine.GetCard(_engine.State.CardsToDraw);
        foreach (var card in cards)
        {
            _engine.GetActivePlayer().PlayerHand.Add(card);
            Console.WriteLine(card);
        }
        _engine.State.CardsToDraw = 0;
        
        _engine.SortPlayerHand(_engine.GetActivePlayer());
        ConsoleVisualization.DrawPlayerHand(_engine.GetActivePlayer());
    }

    private bool? HandleNoPlayableCards()
    {
        bool? continueGame = null;
        Console.WriteLine("\nNo playable cards, press any key to pick a card.");
        Console.ReadKey();
        Console.WriteLine();
        var card = _engine.GetCard(1)[0];
        _engine.GetActivePlayer().PlayerHand.Add(card);
        _engine.SortPlayerHand(_engine.GetActivePlayer());
        Console.WriteLine($"Picked up: {card}");
        if (!_engine.HandHasPlayableCard(_engine.GetActivePlayer().PlayerHand))
        {
            ConsoleVisualization.DrawPlayerHand(_engine.GetActivePlayer());
            Console.WriteLine("Still no playable card, press any key to end turn");
            Console.ReadLine();
            return EndOfTurn();
        }

        return continueGame;
    }

    private void HandleUno()
    {
        var playerCalledUno = ValidateYesNoInput("Do you want to call UNO?");

        if (playerCalledUno)
        {
            if (_engine.ActivePlayerHasOneCard())
            {
                Console.WriteLine("UNO!");
                return;
            }
            Console.WriteLine("You can only call UNO when you have 1 card. Now you must pick up two cards.");
        }

        else if (!playerCalledUno)
        {
            if (_engine.ActivePlayerHasOneCard())
            {
                Console.WriteLine("You have to call UNO when you have one card. Now you must pick up two cards.");
            }
            else return;
        }
        
        var pickedCards = _engine.GetCard(2);
        _engine.GetActivePlayer().PlayerHand.AddRange(pickedCards);
        Console.WriteLine("Picked up two cards:");
        foreach (var card in pickedCards)
        {
            Console.WriteLine(card);
        }
        
    }

    private bool EndOfTurn()
    {
        _engine.NextPlayerTurn();
        _repository.Save(_engine.State.Id, _engine.State);
        var continueGame = ValidateYesNoInput("State saved. Continue playing?");
        return continueGame;
    }

    private ECardColor SelectNewCardColor()
    {
        string newColorStr = ValidateOptionFromListOfStrings(
            "Choose the new color (r - red, g - green, b - blue, y - yellow): ", new List<string>()
            {
                "r", "g", "b", "y"
            });

        ECardColor newColor;
        switch (newColorStr)
        {
            case "r":
                newColor = ECardColor.Red;
                break;
            case "g":
                newColor = ECardColor.Green;
                break;
            case "b":
                newColor = ECardColor.Blue;
                break;
            case "y":
                newColor = ECardColor.Yellow;
                break;
            default:
                throw new InvalidOperationException($"Unexpected color option: {newColorStr}");
        }

        return newColor;
    }
}
