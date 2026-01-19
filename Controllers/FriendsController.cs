using Microsoft.AspNetCore.Mvc;
using HavamaGore.Data;
using HavamaGore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HavamaGore.Controllers
{
    public class FriendsController : Controller
    {
        private readonly AppDbContext _context;

        public FriendsController(AppDbContext context)
        {
            _context = context;
        }

        // ARKADAŞ LİSTESİ SAYFASI
        public async Task<IActionResult> Index()
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return RedirectToAction("Login", "Account");

            // Kabul edilmiş arkadaşlıkları bul
            var friendships = await _context.Friendships
                .Where(f => (f.SenderUsername == myUsername || f.ReceiverUsername == myUsername) && f.IsAccepted)
                .ToListAsync();

            // Arkadaşların kullanıcı adlarını listeye al
            var friendNames = friendships
                .Select(f => f.SenderUsername == myUsername ? f.ReceiverUsername : f.SenderUsername)
                .ToList();

            // Kullanıcı detaylarını çek
            var friends = await _context.Users
                .Where(u => friendNames.Contains(u.Username))
                .ToListAsync();

            return View(friends);
        }

        // ARKADAŞ SİLME
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string username)
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Json(new { success = false });

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.SenderUsername == myUsername && f.ReceiverUsername == username) ||
                    (f.SenderUsername == username && f.ReceiverUsername == myUsername));

            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Arkadaş bulunamadı." });
        }
    }
}