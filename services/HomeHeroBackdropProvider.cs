using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace TheWeekndPort.Services;

/// <summary>
/// Builds a varied hero slideshow: curated Unsplash (film + music, no key) plus optional TMDB trending backdrops.
/// </summary>
public class HomeHeroBackdropProvider
{
    private const string TmdbCacheKey = "home_hero_tmdb_backdrops_v1";
    private static readonly TimeSpan TmdbCacheDuration = TimeSpan.FromMinutes(45);

    /// <summary>Unsplash hotlinks — cinema, concerts, studios (format/crop per Unsplash guidelines).</summary>
    private static readonly string[] UnsplashPool =
    {
        "https://images.unsplash.com/photo-1489599835365-ebbb5c79eaa5?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1478720568477-152d9b164e26?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1517604931442-7e0c8ed29631?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1524985069026-dd778a71c7b4?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1514525253161-7a46d19cd819?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1507679799987-c73779587ccf?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1596727147705-61a532a659bd?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1574267432553-4b4628081c31?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1440409304668-44723763a16b?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1516280440614-379484d1f36d?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1485846234645-a626129f3e36?auto=format&fit=crop&w=1920&q=80",
        "https://images.unsplash.com/photo-1514525537091-18e2c950fa90?auto=format&fit=crop&w=1920&q=80",
    };

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<HomeHeroBackdropProvider> _logger;

    public HomeHeroBackdropProvider(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<HomeHeroBackdropProvider> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> GetSlidesAsync(int desiredCount = 5, CancellationToken cancellationToken = default)
    {
        desiredCount = Math.Clamp(desiredCount, 3, 8);

        var candidates = new List<string>(UnsplashPool);
        var tmdbKey = _configuration["Tmdb:ApiKey"]?.Trim();

        if (!string.IsNullOrEmpty(tmdbKey))
        {
            var fromTmdb = await GetTrendingBackdropUrlsAsync(tmdbKey, cancellationToken).ConfigureAwait(false);
            candidates.AddRange(fromTmdb);
        }

        var distinct = candidates
            .Where(u => Uri.TryCreate(u, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var rnd = Random.Shared;
        var shuffled = distinct.OrderBy(_ => rnd.Next()).ToList();
        var picked = shuffled.Take(desiredCount).ToList();

        while (picked.Count < desiredCount)
        {
            foreach (var url in UnsplashPool.OrderBy(_ => rnd.Next()))
            {
                if (picked.Count >= desiredCount) break;
                if (!picked.Contains(url, StringComparer.OrdinalIgnoreCase))
                    picked.Add(url);
            }

            if (picked.Count == 0)
                break;
        }

        return picked;
    }

    private async Task<IReadOnlyList<string>> GetTrendingBackdropUrlsAsync(string apiKey, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TmdbCacheKey, out IReadOnlyList<string>? cached) && cached is { Count: > 0 })
            return cached;

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(6);

            var url = $"https://api.themoviedb.org/3/trending/movie/week?api_key={Uri.EscapeDataString(apiKey)}";
            using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("TMDB trending request failed: {Status}", response.StatusCode);
                return Array.Empty<string>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);

            var list = new List<string>();
            if (!doc.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
                return Array.Empty<string>();

            foreach (var movie in results.EnumerateArray())
            {
                if (!movie.TryGetProperty("backdrop_path", out var bp) || bp.ValueKind != JsonValueKind.String)
                    continue;
                var path = bp.GetString();
                if (string.IsNullOrWhiteSpace(path)) continue;
                list.Add($"https://image.tmdb.org/t/p/w1280{path}");
                if (list.Count >= 18) break;
            }

            if (list.Count > 0)
                _cache.Set(TmdbCacheKey, list, TmdbCacheDuration);

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "TMDB backdrops unavailable; using Unsplash pool only.");
            return Array.Empty<string>();
        }
    }
}
