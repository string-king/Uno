using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using Domain.Database;

namespace WebApp.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public IndexModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        
        public Dictionary<Game, GameState> GameStates { get;set; } = default!;

        public async Task OnGetAsync()
        {
            GameStates = _gameRepository.GetGamesAndStates();
        }
    }
}
