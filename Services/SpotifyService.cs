using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using HavamaGore.Models; 

namespace HavamaGore.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.Now < _tokenExpiration) return _accessToken;

            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];
            if(string.IsNullOrEmpty(clientId)) return null;

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            var requestBody = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

            try 
            {
                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    _accessToken = doc.RootElement.GetProperty("access_token").GetString();
                    _tokenExpiration = DateTime.Now.AddSeconds(doc.RootElement.GetProperty("expires_in").GetInt32() - 60); 
                    return _accessToken;
                }
            }
            catch { }
            return null;
        }

        public async Task<List<SpotifyPlaylist>> GetPlaylistsByMoodAsync(string mood)
        {
            var token = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token)) return new List<SpotifyPlaylist>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var query = System.Net.WebUtility.UrlEncode($"{mood} music");
            var url = $"https://api.spotify.com/v1/search?q={query}&type=playlist&limit=10";

            try 
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Harf duyarlılığını kaldırarak deserialize ediyoruz
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var searchResult = JsonSerializer.Deserialize<SpotifySearchResponse>(content, options);
                    return searchResult?.Playlists?.Items ?? new List<SpotifyPlaylist>();
                }
            }
            catch { }
            return new List<SpotifyPlaylist>();
        }
    }
}