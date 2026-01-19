using Microsoft.AspNetCore.Mvc;
using HavamaGore.Data;
using HavamaGore.Models;
using Microsoft.EntityFrameworkCore;

namespace HavamaGore.Controllers
{
    public class LibraryController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor'da sadece veritabanı var, Spotify yok!
        public LibraryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login", "Account");

            // Sadece veritabanından çekiyoruz, hata riski yok
            var items = await _context.LibraryItems
                                      .Where(x => x.Username == username)
                                      .OrderByDescending(x => x.CreatedAt)
                                      .ToListAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] LibraryItem item)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return Json(new { success = false, message = "Giriş yapmalısın!" });

            bool exists = await _context.LibraryItems.AnyAsync(x => x.Username == username && x.Title == item.Title && x.Type == item.Type);
            if (exists) return Json(new { success = false, message = "Bu zaten kütüphanende var! 😅" });

            item.Username = username;
            item.CreatedAt = DateTime.Now;

            _context.LibraryItems.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Kütüphaneye eklendi! 📚" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return Json(new { success = false });

            var item = await _context.LibraryItems.FirstOrDefaultAsync(x => x.Id == id && x.Username == username);
            if (item != null)
            {
                _context.LibraryItems.Remove(item);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}