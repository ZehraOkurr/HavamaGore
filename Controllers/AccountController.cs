using Microsoft.AspNetCore.Mvc;
using HavamaGore.Data;
using HavamaGore.Models;
using Microsoft.AspNetCore.Http; // Session için
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;    // Guid için
using System.Threading.Tasks; // Async işlemler için
using System.Linq;
using System.Text.Json; // JSON İşlemleri için eklendi

namespace HavamaGore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. KAYIT OL (REGISTER)
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Username") != null) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                model.Username = model.Username.ToLower().Trim(); 

                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ViewBag.Error = "Bu kullanıcı adı zaten alınmış! 😔";
                    return View(model);
                }

                model.FriendCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                model.ProfilePicture = null; 
                
                // >>> TARİHİ BURADA KESİNLEŞTİRİYORUZ <<<
                model.CreatedAt = DateTime.Now; 

                _context.Users.Add(model);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // ==========================================
        // 2. GİRİŞ YAP (LOGIN)
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Alanları doldurunuz.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username.ToLower().Trim() && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("Username", user.Username);
                
                if(!string.IsNullOrEmpty(user.City))
                {
                    HttpContext.Session.SetString("City", user.City);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı! ❌";
            return View();
        }

        // ==========================================
        // 3. ÇIKIŞ YAP (LOGOUT)
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login");
        }

        // ==========================================
        // 4. PROFİL SAYFASI (Görüntüleme & İstatistikler)
        // ==========================================
       public async Task<IActionResult> Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            // >>> OTO-TAMİR KODU BAŞLANGIÇ <<<
            // Eğer tarih hatalıysa (0001 yılındaysa), bugüne eşitle ve düzelt.
            if (user.CreatedAt.Year == 1) 
            {
                user.CreatedAt = DateTime.Now;
                await _context.SaveChangesAsync(); // Veritabanını güncelle
            }
            // >>> OTO-TAMİR KODU BİTİŞ <<<

            // --- 1. MEVCUT İSTATİSTİKLER ---
            ViewBag.FriendCount = await _context.Friendships
                .Where(f => (f.SenderUsername == username || f.ReceiverUsername == username) && f.IsAccepted)
                .CountAsync();

            ViewBag.LibraryCount = await _context.LibraryItems
                .Where(l => l.Username == username)
                .CountAsync();

            // --- 2. MOOD İSTATİSTİKLERİ ---
            var moodStats = await _context.UserMoodLogs
                .Where(x => x.Username == username)
                .GroupBy(x => x.Mood)
                .Select(g => new { Mood = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.MoodLabels = JsonSerializer.Serialize(moodStats.Select(x => x.Mood.ToUpper()));
            ViewBag.MoodData = JsonSerializer.Serialize(moodStats.Select(x => x.Count));

            var topWeather = await _context.UserMoodLogs
                .Where(x => x.Username == username)
                .GroupBy(x => x.WeatherCondition)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            ViewBag.TopWeather = topWeather ?? "Bilinmiyor";

            return View(user);
        }

        // ==========================================
        // 5. PROFİL RESMİ YÜKLEME (Dosya Upload)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");

            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                var newFileName = $"{username}_{Guid.NewGuid()}{extension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                user.ProfilePicture = $"/uploads/{newFileName}"; 
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile");
        }

        // ==========================================
        // 6. YENİ EKLENEN: KULLANICI ADI GÜNCELLEME
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> UpdateUsername(string newUsername)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            // Eğer yeni isim doluysa ve veritabanında yoksa güncelle
            if (user != null && !string.IsNullOrEmpty(newUsername))
            {
                // Aynı isim var mı kontrolü
                bool exists = await _context.Users.AnyAsync(u => u.Username == newUsername.ToLower().Trim());
                if(exists)
                {
                    // Hata mesajı eklenebilir ama şimdilik Profile dönüyoruz
                    return RedirectToAction("Profile");
                }

                user.Username = newUsername.ToLower().Trim();
                await _context.SaveChangesAsync();
                
                // Session güncelle ki çıkış yapmasın
                HttpContext.Session.SetString("Username", user.Username); 
            }
            return RedirectToAction("Profile");
        }

        // ==========================================
        // 7. YENİ EKLENEN: PROFİL RESMİ GÜNCELLEME (URL İLE)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(string photoUrl)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                user.ProfilePicture = photoUrl;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }

        // ==========================================
        // 8. YENİ EKLENEN: RASTGELE AVATAR
        // ==========================================
        public async Task<IActionResult> UpdatePhotoRandom()
        {
            var username = HttpContext.Session.GetString("Username");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                // UI Avatars servisini kullanarak rastgele avatar ata
                user.ProfilePicture = $"https://ui-avatars.com/api/?name={user.Username}&background=random&size=256&length=1";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }
    }
}