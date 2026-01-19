using Microsoft.EntityFrameworkCore;
using HavamaGore.Models; // Modellerimizi (Tabloları) buradan tanıyacak

namespace HavamaGore.Data
{
    public class AppDbContext : DbContext
    {
        // Bu yapı, veritabanı ayarlarını Program.cs'ten almamızı sağlar
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // İşte SSMS'teki tabloların C# karşılıkları:
        public DbSet<User> Users { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<WatchList> WatchLists { get; set; }
        public DbSet<UserMoodLog> UserMoodLogs { get; set; }
        public DbSet<SearchLog> SearchLogs { get; set; }
    }
}