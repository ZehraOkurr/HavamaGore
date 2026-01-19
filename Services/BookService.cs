using System.Text.Json;
using HavamaGore.Models;

namespace HavamaGore.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GoogleBooksResponse> GetBooksByGenreAsync(string genre)
        {
            try
            {
                // Google Books API (Ücretsiz)
                // q=subject:{genre} -> Türe göre ara
                // langRestrict=tr -> Türkçe kitapları getir
                // orderBy=relevance -> En alakalıları getir
                var url = $"https://www.googleapis.com/books/v1/volumes?q=subject:{genre}&langRestrict=tr&maxResults=8&orderBy=relevance";
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                
                // Google bazen büyük/küçük harf duyarlı olabilir, ayarı gevşetiyoruz
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<GoogleBooksResponse>(json, options);
            }
            catch
            {
                return null; // Hata olursa boş dön
            }
        }
    }
}