namespace TheWeekndPort.Models;

public class HomeIndexViewModel
{
    /// <summary>Background image URLs for the hero slideshow (HTTPS, mixed cinema + music).</summary>
    public IReadOnlyList<string> HeroSlides { get; init; } = Array.Empty<string>();
}
