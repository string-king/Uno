using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Games
{
    public class PlayerSetupModel : PageModel
    {
        private readonly IGameRepository _repository;

        public PlayerSetupModel(IGameRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public int PlayerCount { get; set; }

        [BindProperty]
        public GameOptions Options { get; set; } = default!;

        [BindProperty]
        public GameEngine Engine { get; set; } = default!;
        

        public void OnGet()
        {
            // Your existing OnGet logic
        }

        public void OnPost()
        {
            Engine = new GameEngine(Options);

            for (int i = 1; i <= PlayerCount; i++)
            {
                Engine.State.Players.Add(new Player
                {
                    Nickname = $"Player {i}"
                });
            }
        }

        public void OnPostCreateGame()
        {
            Engine = new GameEngine(Options);

            for (int i = 0; i < PlayerCount; i++)
            {
                    string nickname = Request.Form[$"player.Nickname{i}"]!;
                    EPlayerType playerType = Enum.Parse<EPlayerType>(Request.Form[$"player.PlayerType{i}"]!);
                    Engine.State.Players.Add(new Player()
                    {
                        Nickname = nickname,
                        PlayerType = playerType
                    });
            }



            Engine.InitializeDeckAndHands();
            
            _repository.Save(Engine.State.Id, Engine.State);

            Response.Redirect("/Games/Index");
        }
    }
}