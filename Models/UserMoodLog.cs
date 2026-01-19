using System;
using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class UserMoodLog
    {
        [Key]
        public int Id { get; set; }
        
        public string Username { get; set; } // Hangi kullanıcı?
        public string City { get; set; }     // Hangi şehirde?
        public string WeatherCondition { get; set; } // Hava nasıldı? (Rainy, Sunny vs.)
        public string Mood { get; set; }     // Modu neydi? (Acoustic, Chill vs.)
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ne zaman?
    }
}