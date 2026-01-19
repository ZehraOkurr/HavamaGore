using System;
using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public string Content { get; set; }
        
        // Hatanın sebebi bu satırın eksik olmasıydı:
        public DateTime Timestamp { get; set; } 
        
        public bool IsRead { get; set; }
    }
}