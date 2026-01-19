using Microsoft.AspNetCore.Mvc;
using HavamaGore.Data;
using HavamaGore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace HavamaGore.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        // 1. SOL MENÜ İÇİN ARKADAŞ LİSTESİ (Layout çağırıyor)
        [HttpGet]
        public async Task<IActionResult> GetMyFriendsAPI()
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Unauthorized();

            // Arkadaşlıkları bul (Hem gönderen hem alan olabilirim)
            var friendships = await _context.Friendships
                .Where(f => (f.SenderUsername == myUsername || f.ReceiverUsername == myUsername) && f.IsAccepted)
                .ToListAsync();

            // Arkadaşların isimlerini listeye al
            var friendNames = friendships
                .Select(f => f.SenderUsername == myUsername ? f.ReceiverUsername : f.SenderUsername)
                .ToList();

            // Arkadaş detaylarını ve okunmamış mesaj sayılarını çek
            var friends = await _context.Users
                .Where(u => friendNames.Contains(u.Username))
                .Select(u => new { 
                    u.Username, 
                    u.ProfilePicture,
                    // Bana gelen ve henüz okumadığım mesajların sayısı
                    UnreadCount = _context.Messages.Count(m => m.SenderUsername == u.Username && m.ReceiverUsername == myUsername && !m.IsRead)
                })
                .ToListAsync();

            return Json(friends);
        }

        // 2. MESAJLARI GETİR (Sohbet açılınca)
        [HttpGet]
        public async Task<IActionResult> GetMessages(string username)
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Unauthorized();

            var messages = await _context.Messages
                .Where(m => (m.SenderUsername == myUsername && m.ReceiverUsername == username) ||
                            (m.SenderUsername == username && m.ReceiverUsername == myUsername))
                .OrderBy(m => m.Timestamp) // Tarihe göre sırala
                .ToListAsync();

            return Json(messages);
        }

        // 3. MESAJ GÖNDER
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessagePayload payload)
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Unauthorized();

            if (string.IsNullOrEmpty(payload.Content)) return BadRequest("Mesaj boş olamaz");

            var msg = new Message
            {
                SenderUsername = myUsername,
                ReceiverUsername = payload.ReceiverUsername,
                Content = payload.Content,
                Timestamp = DateTime.Now,
                IsRead = false // Yeni mesaj okunmamış olarak gider
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // 4. OKUNDU OLARAK İŞARETLE (Bildirimi silmek için)
        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsReadAPI(string sender)
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Unauthorized();

            // O kişiden bana gelen ve okunmamış mesajları bul
            var unreadMsgs = await _context.Messages
                .Where(m => m.SenderUsername == sender && m.ReceiverUsername == myUsername && !m.IsRead)
                .ToListAsync();

            if (unreadMsgs.Any())
            {
                foreach (var msg in unreadMsgs) 
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        // 5. TOPLAM OKUNMAMIŞ SAYISI (Genel kırmızı bildirim Badge'i için)
        [HttpGet]
        public async Task<IActionResult> GetTotalUnreadCountAPI()
        {
            var myUsername = HttpContext.Session.GetString("Username");
            if (myUsername == null) return Json(0);

            var count = await _context.Messages
                .CountAsync(m => m.ReceiverUsername == myUsername && !m.IsRead);

            return Json(count);
        }

        // Yardımcı Sınıf
        public class MessagePayload
        {
            public string ReceiverUsername { get; set; }
            public string Content { get; set; }
        }
    }
}