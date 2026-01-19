using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavamaGore.Models
{
    public class WatchList
    {
        [Key]
        public int ListID { get; set; }

        public int UserID { get; set; }

        // Bu satır veritabanında ilişki olduğunu koda anlatır
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public string MovieTitle { get; set; }

        public string PosterPath { get; set; }

        public DateTime AddedDate { get; set; }
    }
}