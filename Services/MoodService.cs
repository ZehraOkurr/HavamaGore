using System.Text.Json;
using HavamaGore.Models;

namespace HavamaGore.Services
{
    public class MoodService
    {
        private readonly HttpClient _httpClient;

        // API KEYLERİNİ BURAYA YAZ (Tırnakların içine)
        private const string WeatherApiKey = "1c24eabc0c7c4bf8829115135251812"; 
        private const string TmdbApiKey = "bdd6239aef7127b13ec91955ad702322";

        public MoodService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. HAVA DURUMUNU GETİR
        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            // WeatherAPI.com'a istek atıyoruz
            string url = $"http://api.weatherapi.com/v1/current.json?key={WeatherApiKey}&q={city}&lang=tr";
            
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WeatherResponse>(jsonString);
            }
            return null; // Hata olursa boş dön
        }

        // 2. MODA GÖRE FİLM GETİR
        public async Task<MovieResponse> GetMoviesByGenreAsync(int genreId)
        {
            // TMDB'ye istek atıyoruz
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={TmdbApiKey}&with_genres={genreId}&language=tr-TR&sort_by=popularity.desc";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MovieResponse>(jsonString);
            }
            return null;
        }
    }
}