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
    public class DetailsModel : PageModel
    {
        private readonly IGameRepository _repository;

        public DetailsModel(IGameRepository repository)
        {
            _repository = repository;
        }

        public Game Game { get; set; } = default!;
        public GameState State { get; set; }  = default!;
        public ICollection<Player> Players { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // var game = await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
            
            var games = _repository.GetGamesAndStates();
            var game = games.FirstOrDefault(g => g.Key.Id == id).Key;
            State = _repository.LoadGame((Guid)id);
            if (game.Id == null)
            {
                // return NotFound();
            }
            else
            {
                Game = game;
                Players = State.Players;
            }
            return Page();
        }
    }
}
