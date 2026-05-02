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

    
    public IActionResult Index()
    {
        var artists = _context.Artists.ToList();
        return View(artists);
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