using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeekndPort.Data;

public class ArtistsController : Controller
{
    private readonly AppDbContext _context;

    public ArtistsController(AppDbContext context)
    {
        _context = context;
    }


    public IActionResult Index(string searchTerm, string eraFilter)
    {
        var artists = _context.Artists
            .Include(a => a.Albums)
            .AsQueryable();

       
        if (!string.IsNullOrEmpty(searchTerm))
        {
            artists = artists.Where(a =>
                a.Name.Contains(searchTerm) ||
                a.Albums.Any(al => al.Title.Contains(searchTerm))
            );
        }

        
        if (!string.IsNullOrEmpty(eraFilter))
        {
            artists = artists.Where(a =>
                a.Albums.Any(al =>
                    al.Era.ToLower() == eraFilter.ToLower()
                )
            );
        }

        return View(artists.ToList());
    }

    [HttpGet]
    public JsonResult SearchSuggestions(string term)
    {
        var results = _context.Artists
            .Include(a => a.Albums)
            .Where(a =>
                a.Name.Contains(term) ||
                a.Albums.Any(al => al.Title.Contains(term))
            )
           .Select(a => new
           {
               id = a.Id,
               artist = a.Name,
               albums = a.Albums.Select(al => al.Title)
           })
            .Take(5)
            .ToList();

        return Json(results);
    }


    public IActionResult Details(int id)
    {
        var artist = _context.Artists
            .Include(a => a.Albums)
                .ThenInclude(al => al.Tracks)
            .FirstOrDefault(a => a.Id == id);

        return View(artist);
    }

    
    public IActionResult Timeline(int id)
    {
        var artist = _context.Artists
            .Include(a => a.Albums)
                .ThenInclude(al => al.Inspirations)
            .FirstOrDefault(a => a.Id == id);

        return View(artist);
    }
}