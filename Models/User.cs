using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class User
    {
        [Key] // Bu satır, bunun Primary Key olduğunu belirtir
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string FriendCode { get; set; } // Örn: "8492-XYE"
        public string ProfilePicture { get; set; } // Resim dosya yolu (Örn: /uploads/resim.jpg)
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Kayıt Tarihi (Hata veren kısım buydu)
        public double? Latitude { get; set; }  // Enlem
        public double? Longitude { get; set; } // Boylam
        public DateTime? LastLocationUpdate { get; set; } // Ne zaman görüldü?
        public bool IsLocationVisible { get; set; } = true; // Hayalet Modu için (İleride kapatabilsin diye)


    }
}