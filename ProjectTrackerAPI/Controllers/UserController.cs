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

        // SHA256 algoritmasý ile düz metin parolayý hash'ler
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // TÜM KULLANICILARI GETÝR (Yalnýzca Admin)
        // GET: /api/user
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            return Ok(_context.Users.ToList());
        }

        // KULLANICI SÝL (Yalnýzca Admin)
        // DELETE: /api/user/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            // Silinecek kullanýcýyý bul
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Kullanýcý bulunamadý.");

            // Eðer kullanýcýya atanmýþ görev varsa silmesine izin verilmez
            var hasAssignedTodos = _context.Todos.Any(t => t.AssignedUserId == id);
            if (hasAssignedTodos)
                return BadRequest("Kullanýcýya atanmýþ görev(ler) var. Silmeden önce görevleri kaldýrmalýsýnýz.");

            // Kullanýcýyý veritabanýndan sil
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("Kullanýcý silindi.");
        }

        // ATANABÝLÝR KULLANICILARI GETÝR (Admin tüm kullanýcýlarý alýr, Manager sadece Employee rolündekileri)
        // GET: /api/user/assignable
        [HttpGet("assignable")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult GetAssignableUsers()
        {
            // JWT token'dan kullanýcýnýn rolünü al
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
                return Ok(_context.Users.ToList());

            if (role == "Manager")
                return Ok(_context.Users.Where(u => u.Role == "Employee").ToList());

            return Forbid("Yetersiz yetki.");
        }

        // KULLANICI GÜNCELLE (Yalnýzca Admin)
        // PUT: /api/user/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateUser(int id, User updated)
        {
            // Güncellenecek kullanýcýyý bul
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Kullanýcý bulunamadý.");

            // Bilgileri güncelle
            user.Name = updated.Name;
            user.Email = updated.Email;
            user.Role = updated.Role;

            // Eðer yeni þifre geldiyse hash'leyerek güncelle
            if (!string.IsNullOrWhiteSpace(updated.Password))
            {
                user.PasswordHash = HashPassword(updated.Password);
            }

            // Deðiþiklikleri veritabanýna kaydet
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
