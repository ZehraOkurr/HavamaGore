using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        public string SenderUsername { get; set; }   // İsteği atan (Sen)
        public string ReceiverUsername { get; set; } // İsteği alan (Arkadaşın)
        
        public bool IsAccepted { get; set; } = false; // Kabul edildi mi? (İlk başta false)
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}