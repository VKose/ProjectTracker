using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // SHA256 algoritmas� ile d�z metin parolay� hash'ler
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // T�M KULLANICILARI GET�R (Yaln�zca Admin)
        // GET: /api/user
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            return Ok(_context.Users.ToList());
        }

        // KULLANICI S�L (Yaln�zca Admin)
        // DELETE: /api/user/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            // Silinecek kullan�c�y� bul
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Kullan�c� bulunamad�.");

            // E�er kullan�c�ya atanm�� g�rev varsa silmesine izin verilmez
            var hasAssignedTodos = _context.Todos.Any(t => t.AssignedUserId == id);
            if (hasAssignedTodos)
                return BadRequest("Kullan�c�ya atanm�� g�rev(ler) var. Silmeden �nce g�revleri kald�rmal�s�n�z.");

            // Kullan�c�y� veritaban�ndan sil
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("Kullan�c� silindi.");
        }

        // ATANAB�L�R KULLANICILARI GET�R (Admin t�m kullan�c�lar� al�r, Manager sadece Employee rol�ndekileri)
        // GET: /api/user/assignable
        [HttpGet("assignable")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult GetAssignableUsers()
        {
            // JWT token'dan kullan�c�n�n rol�n� al
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
                return Ok(_context.Users.ToList());

            if (role == "Manager")
                return Ok(_context.Users.Where(u => u.Role == "Employee").ToList());

            return Forbid("Yetersiz yetki.");
        }

        // KULLANICI G�NCELLE (Yaln�zca Admin)
        // PUT: /api/user/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(int id, User updated)
        {
            // G�ncellenecek kullan�c�y� bul
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Kullan�c� bulunamad�.");

            // Bilgileri g�ncelle
            user.Name = updated.Name;
            user.Email = updated.Email;
            user.Role = updated.Role;

            // E�er yeni �ifre geldiyse hash'leyerek g�ncelle
            if (!string.IsNullOrWhiteSpace(updated.Password))
            {
                user.PasswordHash = HashPassword(updated.Password);
            }

            // De�i�iklikleri veritaban�na kaydet
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
