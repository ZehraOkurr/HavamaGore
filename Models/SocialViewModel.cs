using System;
using System.Collections.Generic;

namespace HavamaGore.Models
{
    public class SocialViewModel
    {
        // Önerilen Rastgele Kullanıcılar (Arkadaş Ekleme İçin)
        public List<User> SuggestedUsers { get; set; } = new List<User>();

        // Arkadaşların Son Aktiviteleri (Feed)
        public List<ActivityFeedItem> Feed { get; set; } = new List<ActivityFeedItem>();
    }

    public class ActivityFeedItem
    {
        public string Username { get; set; }
        public string ProfilePicture { get; set; }
        public string Mood { get; set; }           // "Chill", "Party" vb.
        public string Weather { get; set; }        // "Rainy", "Sunny" vb.
        public string City { get; set; }
        public DateTime Time { get; set; }
        public string TimeAgo => GetTimeAgo(Time);

        // Zamanı "2 saat önce" şeklinde göstermek için yardımcı metod
        private string GetTimeAgo(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalMinutes < 1) return "Az önce";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} dk önce";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} saat önce";
            return $"{(int)span.TotalDays} gün önce";
        }
    }
}