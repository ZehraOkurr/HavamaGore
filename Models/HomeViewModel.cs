using System.Collections.Generic;

namespace HavamaGore.Models
{
    public class HomeViewModel
    {
        public string City { get; set; }
        public string RecommendedGenreName { get; set; }

        public WeatherResponse Weather { get; set; }
        public MovieResponse Movies { get; set; }
        public GoogleBooksResponse Books { get; set; }
        
        // HATA ÇÖZÜMÜ: Burası List<SpotifyPlaylist> olmalı
        public List<SpotifyPlaylist> Musics { get; set; } = new List<SpotifyPlaylist>();
    }
}