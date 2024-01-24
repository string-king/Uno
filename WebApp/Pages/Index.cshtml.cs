using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IGameRepository _repository;
    public IndexModel(IGameRepository repository)
    {
        _repository = repository;
    }

    public int Count { get; set; }
    
    public void OnGet()
    {
        Count = _repository.GetSaveGames().Count;
    }

    
}