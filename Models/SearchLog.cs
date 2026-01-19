using System;
using System.ComponentModel.DataAnnotations;

namespace HavamaGore.Models
{
    public class SearchLog
    {
        [Key]
        public int LogID { get; set; }

        public int UserID { get; set; }

        public string WeatherCondition { get; set; }

        public string RecommendedGenre { get; set; }

        public DateTime SearchDate { get; set; }
    }
}