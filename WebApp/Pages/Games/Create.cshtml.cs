using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using Domain.Database;

namespace WebApp.Pages.Games
{
    public class CreateModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public CreateModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        
        [BindProperty, Range(2, 10, ErrorMessage = "The value must be between 2 and 10.")]
        public int PlayerCount { get; set; }

        [BindProperty]
        public GameOptions Options { get; set; } = new GameOptions();


        public IActionResult OnGet()
        {
            return Page();
        }
        
    }
}
