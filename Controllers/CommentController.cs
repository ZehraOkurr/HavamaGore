using Microsoft.AspNetCore.Mvc;
using HavamaGore.Models;
using HavamaGore.Data;
using Microsoft.EntityFrameworkCore;

namespace HavamaGore.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Yorumları Getir (Bir film/kitap açıldığında çalışır)
        [HttpGet]
        public async Task<IActionResult> GetComments(string externalId)
        {
            var comments = await _context.Comments
                                         .Where(x => x.ExternalId == externalId)
                                         .OrderByDescending(x => x.CreatedAt)
                                         .ToListAsync();
            return Json(comments);
        }

        // 2. Yorum Yap (Gönder butonuna basınca çalışır)
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return Json(new { success = false, message = "Önce giriş yapmalısın! 🔐" });

            if (string.IsNullOrEmpty(comment.Content)) return Json(new { success = false, message = "Boş yorum atamazsın! 😅" });

            comment.Username = username;
            comment.CreatedAt = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Yorumun eklendi! 💬" });
        }
    }
}