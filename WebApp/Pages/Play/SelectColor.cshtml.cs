using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.TextTemplating;
using UnoEngine;

namespace WebApp.Pages.Play;

public class SelectColor : PageModel
{
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public Guid PlayerId { get; set; }
    

    public void OnGet()
    {
       
    }
}