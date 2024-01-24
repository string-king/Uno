using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Play;

public class Index : PageModel
{

    private readonly IGameRepository _repository;
    public Player CurrentPlayer { get; set; } = default!;
    public GameEngine Engine { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)] public Guid? PlayerId { get; set; } = null;
    
    [BindProperty(SupportsGet = true)]
    public int CardNr { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public bool UnoCalled { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public EWebMessage Message { get; set; } = EWebMessage.None;

    public int DrawAmount { get; set; }
    
    

    public Index(IGameRepository repository)
    {
        _repository = repository;
    }
    
    public void OnGet()
    {
        InitializeValues();

        if (Engine.State.GameOver)
        {
            Response.Redirect("/Play/GameOver?GameId=" + GameId);
            return;
        }

        if (Engine.GetActivePlayer().PlayerType == EPlayerType.AI)
        {
            HandleAiTurn();
            return;
        }
        
        if (Engine.GetActivePlayer().Id == PlayerId)
        {
            if (Engine.State.CardsToDraw > 0)
            {
                if (Engine.State.Forwarding == false || !Engine.HandHasPlayableCard(Engine.GetActivePlayer().PlayerHand))
                {
                    Engine.GetActivePlayerHand().AddRange(Engine.GetCard(Engine.State.CardsToDraw));
                    DrawAmount = Engine.State.CardsToDraw;
                    Engine.State.CardsToDraw = 0;
                    Message = EWebMessage.PickedUpCards;
                    Engine.NextPlayerTurn();
                }
            }
            else if (!Engine.HandHasPlayableCard(Engine.GetActivePlayerHand()))
            {
                Engine.GetActivePlayerHand().AddRange(Engine.GetCard(1));
                if (!Engine.HandHasPlayableCard(Engine.GetActivePlayerHand()))
                {
                    Message = EWebMessage.NoPlayableCard;
                    Engine.NextPlayerTurn();
                }
                else
                {
                    Message = EWebMessage.PickedUpACard;
                }
            }
        }
        _repository.Save(GameId, Engine.State);

        if (PlayerId != null)
        {
            Engine.SortPlayerHand(CurrentPlayer);
        }
        
    }
    public async Task<IActionResult> OnGetPlayCard()
    {
        InitializeValues();
        var card = Engine.GetActivePlayer().PlayerHand[CardNr];

        if (Engine.GetActivePlayerHand().Count == 2 && UnoCalled == false)
        {
            Engine.GetActivePlayerHand().AddRange(Engine.GetCard(2));
            Message = EWebMessage.UnoFail;
        }
        else
        {
            Engine.PlayCard(card);
            if (Engine.GetActivePlayerHand().Count == 0)
            {
                Engine.State.GameOver = true;
                Engine.State.Winner = Engine.GetActivePlayer();
                _repository.Save(GameId, Engine.State);
                return RedirectToPage("/Play/GameOver", new { GameId });
            }
            Engine.NextPlayerTurn();
        }
        
        _repository.Save(GameId, Engine.State);
        if (card.Color == ECardColor.Wild)
        {
            return RedirectToPage("/Play/SelectColor", new { GameId, PlayerId });
        }
        return RedirectToPage("/Play/Index", new { GameId, PlayerId, Message, UnoCalled });
    }

    
    public async Task<IActionResult> OnGetCallUno()
    {
        InitializeValues();
        if (Engine.GetActivePlayerHand().Count != 2)
        {
            Engine.GetActivePlayerHand().AddRange(Engine.GetCard(2));
            Message = EWebMessage.UnoFail;
        }
        else
        {
            UnoCalled = true;
            Message = EWebMessage.UnoSuccess;
        }

        _repository.Save(GameId, Engine.State);

        return RedirectToPage("/Play/Index", new { GameId, PlayerId, Message, UnoCalled });
    }


    [BindProperty(SupportsGet = true)] public string Color { get; set; } = default!;
    public async Task<IActionResult> OnGetChangeColor(Guid gameId, Guid playerId, string color)
    {
        InitializeValues();

        switch (color)
        {
            case "Red":
            {
                Engine.State.CardInPlay.SetColor(ECardColor.Red);
                break;
            }
            case "Green":
            {
                Engine.State.CardInPlay.SetColor(ECardColor.Green);
                break;
            }
            case "Blue":
            {
                Engine.State.CardInPlay.SetColor(ECardColor.Blue);
                break;
            }
            case "Yellow":
            {
                Engine.State.CardInPlay.SetColor(ECardColor.Yellow);
                break;
            }
        }
        
        _repository.Save(GameId, Engine.State);
        
        return RedirectToPage("/Play/Index", new { GameId = gameId, PlayerId = playerId, Message = EWebMessage.ColorChanged });
    }

    private void HandleAiTurn()
    {
        if (Engine.State.CardsToDraw > 0)
        {
            if (Engine.State.Forwarding  && Engine.HandHasPlayableCard(Engine.GetActivePlayerHand()))
            {
                Engine.AiPlayCard();
            }
            else
            {
                Engine.GetActivePlayer().PlayerHand.AddRange(Engine.GetCard(Engine.State.CardsToDraw));
                Engine.State.CardsToDraw = 0;                
            }
        }
        else if (!Engine.HandHasPlayableCard(Engine.GetActivePlayerHand()))
        {
            Engine.GetActivePlayerHand().Add(Engine.GetCard(1).First());
        }

        else
        {
            Engine.AiPlayCard();
        }
        
        if (Engine.GetActivePlayerHand().Count == 0)
        {
            Engine.State.GameOver = true;
            Engine.State.Winner = Engine.GetActivePlayer();
            _repository.Save(GameId, Engine.State);
            Response.Redirect("/Play/GameOver?GameId=" + GameId);
            return;
        }
        Engine.NextPlayerTurn();
        _repository.Save(GameId, Engine.State);
    }

    private void InitializeValues()
    {
        var gameState = _repository.LoadGame(GameId);
        Engine = new GameEngine
        {
            State = gameState
        };
        if (PlayerId != null)
        {
            CurrentPlayer = Engine.State.Players.FirstOrDefault(p => p.Id == PlayerId);
        }
        
    }
}