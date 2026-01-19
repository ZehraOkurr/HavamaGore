using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HavamaGore.Models;
using HavamaGore.Services;
using System.Text.Json;
using HavamaGore.Data;
using Microsoft.EntityFrameworkCore;

namespace HavamaGore.Controllers;

public class HomeController : Controller
{
    private readonly WeatherService _weatherService;
    private readonly SpotifyService _spotifyService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public HomeController(WeatherService weatherService, SpotifyService spotifyService, AppDbContext context, IConfiguration configuration, HttpClient httpClient)
    {
        _weatherService = weatherService;
        _spotifyService = spotifyService;
        _context = context;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index(string city)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(city))
        {
            city = "Istanbul";
            if (username != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null && !string.IsNullOrEmpty(user.City)) city = user.City;
            }
        }

        var weatherData = await _weatherService.GetWeatherAsync(city);
        
        string mood = "pop";
        string movieGenre = "28"; 
        string bookQuery = "adventure";
        string genreName = "Karışık";

        if (weatherData != null)
        {
            var condition = weatherData.Current?.Condition?.Text?.ToLower() ?? "";
            
            if (condition.Contains("rain") || condition.Contains("yağmur")) { mood = "acoustic"; movieGenre = "18"; bookQuery = "poetry"; genreName = "Melankolik & Huzurlu"; }
            else if (condition.Contains("cloud") || condition.Contains("bulut")) { mood = "chill"; movieGenre = "9648"; bookQuery = "mystery"; genreName = "Gizemli & Sakin"; }
            else if (condition.Contains("clear") || condition.Contains("sunny")) { mood = "summer"; movieGenre = "35"; bookQuery = "comedy"; genreName = "Enerjik & Neşeli"; }
            else if (condition.Contains("snow") || condition.Contains("kar")) { mood = "piano"; movieGenre = "10749"; bookQuery = "romance"; genreName = "Romantik & Soğuk"; }
        }

        var spotifyData = await _spotifyService.GetPlaylistsByMoodAsync(mood);
        var movieData = await GetMoviesAsync(movieGenre);
        var bookData = await GetBooksAsync(bookQuery);

        var viewModel = new HomeViewModel
        {
            City = city,
            Weather = weatherData,
            Musics = spotifyData, // List<SpotifyPlaylist>
            Movies = movieData,
            Books = bookData,
            RecommendedGenreName = genreName
        };

        await LogUserMood(city, weatherData?.Current?.Condition?.Text, mood);

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyProposal()
    {
        string[] moods = { "acoustic", "chill", "summer", "piano", "rock" };
        string[] movieGenres = { "18", "9648", "35", "10749", "28" };
        string[] bookQueries = { "poetry", "mystery", "comedy", "romance", "adventure" };
        
        Random rnd = new Random();
        int index = rnd.Next(moods.Length);

        var spotifyData = await _spotifyService.GetPlaylistsByMoodAsync(moods[index]);
        var movieData = await GetMoviesAsync(movieGenres[index]);
        var bookData = await GetBooksAsync(bookQueries[index]);

        var randomMovie = movieData?.Results?.OrderBy(x => rnd.Next()).FirstOrDefault();
        var randomBook = bookData?.Items?.OrderBy(x => rnd.Next()).FirstOrDefault();
        var randomMusic = spotifyData?.OrderBy(x => rnd.Next()).FirstOrDefault();

        var proposal = new 
        {
            Mood = moods[index].ToUpper(),
            MovieTitle = randomMovie?.Title ?? "Film Bulunamadı",
            MovieImg = "https://image.tmdb.org/t/p/w500" + randomMovie?.PosterPath,
            MovieDesc = randomMovie?.Overview,
            BookTitle = randomBook?.VolumeInfo?.Title ?? "Kitap Bulunamadı",
            BookImg = randomBook?.VolumeInfo?.ImageLinks?.Thumbnail ?? "https://via.placeholder.com/150",
            BookAuthor = randomBook?.VolumeInfo?.Authors != null ? string.Join(", ", randomBook.VolumeInfo.Authors) : "Yazar Yok",
            MusicName = randomMusic?.Name ?? "Liste Bulunamadı",
            MusicImg = randomMusic?.Images?.FirstOrDefault()?.Url ?? "https://via.placeholder.com/150",
            MusicLink = randomMusic?.ExternalUrls?.Spotify ?? "#"
        };

        return Json(proposal);
    }

    private async Task<MovieResponse> GetMoviesAsync(string genreId)
    {
        var apiKey = _configuration["TMDB:ApiKey"];
        var url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genreId}&sort_by=popularity.desc&language=tr-TR";
        try {
            var response = await _httpClient.GetAsync(url);
            if(response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MovieResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        } catch {}
        return new MovieResponse();
    }

    private async Task<GoogleBooksResponse> GetBooksAsync(string query)
    {
        var url = $"https://www.googleapis.com/books/v1/volumes?q={query}&langRestrict=tr&maxResults=10&orderBy=relevance";
        try {
            var response = await _httpClient.GetAsync(url);
            if(response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GoogleBooksResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        } catch {}
        return new GoogleBooksResponse();
    }

    // Loglama Metodu (Veritabanına Kaydeder)
    private async Task LogUserMood(string city, string weather, string mood)
    {
        var username = HttpContext.Session.GetString("Username");
        if (!string.IsNullOrEmpty(username))
        {
            var log = new UserMoodLog
            {
                Username = username,
                City = city,
                WeatherCondition = weather,
                Mood = mood,
                CreatedAt = DateTime.Now
            };
            _context.UserMoodLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}