using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheWeekndPort.Models;
using TheWeekndPort.Services;

namespace TheWeekndPort.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeHeroBackdropProvider _heroBackdropProvider;

        public HomeController(ILogger<HomeController> logger, HomeHeroBackdropProvider heroBackdropProvider)
        {
            _logger = logger;
            _heroBackdropProvider = heroBackdropProvider;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var slides = await _heroBackdropProvider.GetSlidesAsync(5, cancellationToken).ConfigureAwait(false);
            return View(new HomeIndexViewModel { HeroSlides = slides });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
