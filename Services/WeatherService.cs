using System.Text.Json;
using HavamaGore.Models;

namespace HavamaGore.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            var apiKey = _configuration["WeatherAPI:ApiKey"];
            // Metric = Santigrat derece için
            var url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&lang=tr";
            
            // NOT: Eğer WeatherAPI.com kullanıyorsan üstteki, OpenWeatherMap kullanıyorsan format farklıdır.
            // Biz burada WeatherAPI.com formatına uygun basit bir model kullandık.
            // Ücretsiz anahtar için: weatherapi.com (Çok daha basit)
            
            // Eğer OpenWeatherMap kullanacaksan url şöyle olur:
            // var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=tr";

            try 
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WeatherResponse>(json);
                }
            }
            catch { /* Hata olursa null dön */ }
            return null;
        }
    }
}