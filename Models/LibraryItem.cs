using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class LibraryItem
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } // Kim kaydetti?

        public string Title { get; set; }    // Başlık (Film/Şarkı Adı)
        public string Type { get; set; }     // Türü: "Movie", "Music", "Book"
        public string ImageUrl { get; set; } // Kapak Resmi
        public string Overview { get; set; } // Özet / Açıklama
        
        public string ExternalLink { get; set; } // Spotify Linki vb.
        public string ExternalId { get; set; }   // TMDB ID veya Spotify ID

        public int UserRating { get; set; }  // Senin verdiğin puan (1-5)
        public string UserComment { get; set; } // Senin yorumun
        
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ne zaman ekledin?
    }
}