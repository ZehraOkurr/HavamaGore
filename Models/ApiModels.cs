using System.Collections.Generic;
using System.Text.Json.Serialization; 

namespace HavamaGore.Models
{
    // 1. HAVA DURUMU
    public class WeatherResponse
    {
        [JsonPropertyName("location")] public WeatherLocation Location { get; set; }
        [JsonPropertyName("current")] public WeatherCurrent Current { get; set; }
    }
    public class WeatherLocation { [JsonPropertyName("name")] public string Name { get; set; } }
    public class WeatherCurrent { 
        [JsonPropertyName("temp_c")] public double TempC { get; set; } 
        [JsonPropertyName("condition")] public WeatherCondition Condition { get; set; }
    }
    public class WeatherCondition { 
        [JsonPropertyName("text")] public string Text { get; set; } 
        [JsonPropertyName("icon")] public string Icon { get; set; }
    }

    // 2. FİLMLER
    public class MovieResponse
    {
        [JsonPropertyName("results")] public List<MovieItem> Results { get; set; }
    }
    public class MovieItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("overview")] public string Overview { get; set; }
        [JsonPropertyName("poster_path")] public string PosterPath { get; set; }
        [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
    }

    // 3. KİTAPLAR
    public class GoogleBooksResponse
    {
        [JsonPropertyName("items")] public List<BookItem> Items { get; set; }
    }
    public class BookItem
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("volumeInfo")] public BookVolumeInfo VolumeInfo { get; set; }
    }
    public class BookVolumeInfo
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("authors")] public List<string> Authors { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("imageLinks")] public BookImageLinks ImageLinks { get; set; }
        [JsonPropertyName("previewLink")] public string PreviewLink { get; set; }
        [JsonPropertyName("pageCount")] public int? PageCount { get; set; }
    }
    public class BookImageLinks { [JsonPropertyName("thumbnail")] public string Thumbnail { get; set; } }

    // 4. SPOTIFY (MÜZİK)
    public class SpotifySearchResponse
    {
        [JsonPropertyName("playlists")] public SpotifyPlaylistResult Playlists { get; set; }
    }
    public class SpotifyPlaylistResult
    {
        [JsonPropertyName("items")] public List<SpotifyPlaylist> Items { get; set; }
    }
    public class SpotifyPlaylist
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("images")] public List<SpotifyImage> Images { get; set; }
        [JsonPropertyName("external_urls")] public SpotifyExternalUrls ExternalUrls { get; set; }
    }
    public class SpotifyImage { [JsonPropertyName("url")] public string Url { get; set; } }
    public class SpotifyExternalUrls { [JsonPropertyName("spotify")] public string Spotify { get; set; } }
}