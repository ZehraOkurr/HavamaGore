using Microsoft.AspNetCore.Mvc;
using HavamaGore.Data;
using HavamaGore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HavamaGore.Controllers
{
    public class SocialController : Controller
    {
        private readonly AppDbContext _context;

        public SocialController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return RedirectToAction("Login", "Account");

            var viewModel = new SocialViewModel();

            // 1. ARKADAŞ LİSTESİNİ BUL (Feed İçin Lazım)
            var friendships = await _context.Friendships
                .Where(f => (f.SenderUsername == myUsername || f.ReceiverUsername == myUsername) && f.IsAccepted)
                .ToListAsync();

            var friendUsernames = friendships
                .Select(f => f.SenderUsername == myUsername ? f.ReceiverUsername : f.SenderUsername)
                .ToList();

            // 2. AKTİVİTE AKIŞI (FEED) - Arkadaşların son moodlarını çek
            // Not: En son 24 saatteki moodlar
            var logs = await _context.UserMoodLogs
                .Where(l => friendUsernames.Contains(l.Username))
                .OrderByDescending(l => l.CreatedAt)
                .Take(20) // En son 20 aktivite
                .ToListAsync();

            // Logları FeedItem'a çevir (Profil resmini almak için User tablosuna bakmamız lazım)
            foreach (var log in logs)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == log.Username);
                viewModel.Feed.Add(new ActivityFeedItem
                {
                    Username = log.Username,
                    ProfilePicture = user?.ProfilePicture ?? $"https://ui-avatars.com/api/?name={log.Username}&background=random",
                    Mood = log.Mood,
                    Weather = log.WeatherCondition,
                    City = log.City,
                    Time = log.CreatedAt
                });
            }

            // 3. ARKADAŞ ÖNERİLERİ (Zaten arkadaş olmadığın ve sen olmayanlar)
            // Listede olmayanları bul
            var allExcluded = new List<string>(friendUsernames) { myUsername };
            
            viewModel.SuggestedUsers = await _context.Users
                .Where(u => !allExcluded.Contains(u.Username))
                .OrderBy(r => Guid.NewGuid()) // Rastgele getir
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }
    }
}