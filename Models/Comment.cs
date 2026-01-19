using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } // Yorumu kim yaptı?
        
        public string ExternalId { get; set; } // Hangi Filme/Kitaba yapıldı? (ID'si)
        public string Type { get; set; }       // Türü: "Movie", "Music", "Book"
        
        public string Content { get; set; }    // Yorum metni
        public int Rating { get; set; }        // Puan (1-5)
        
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ne zaman?
    }
}