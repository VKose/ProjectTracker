using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.DTOs;
using ProjectTrackerAPI.Helpers;
using ProjectTrackerAPI.Models;
using System.Security.Claims;

namespace ProjectTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        // Kullanýcý Kaydý (Yetkisiz kullanýcýlar eriþebilir)
        // POST: /api/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // Token gerektirmez
        public IActionResult Register(UserRegisterDto dto)
        {
            // Ayný e-posta ile kayýtlý kullanýcý var mý kontrol et
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta zaten kayýtlý.");

            // Yeni kullanýcý oluþtur
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kayýt baþarýlý.");
        }

        // Giriþ Ýþlemi (Yetkisiz kullanýcýlar eriþebilir)
        // POST: /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous] // Token gerektirmez
        public IActionResult Login(UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            // Kullanýcý yoksa ya da þifre yanlýþsa hata ver
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("E-posta veya þifre hatalý.");

            // Token üret ve gönder
            var token = _jwtHelper.GenerateToken(user);
            return Ok(new { token });
        }

        // Þifre Deðiþtir (Sadece yetkili kullanýcýlar eriþebilir)
        // PUT: /api/auth/change-password/{id}
        [HttpPut("change-password/{id}")]
        [Authorize] // Giriþ yapýlmýþ olmalý
        public IActionResult ChangePassword(int id, [FromBody] string newPassword)
        {
            // Token'dan giriþ yapan kiþinin ID'sini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Token'dan ID alýnamazsa veya parse edilemezse hata
            if (!int.TryParse(userIdClaim, out int userIdFromToken))
                return Unauthorized("Geçersiz kullanýcý kimliði.");

            // Admin mi kontrol et
            var isAdmin = User.IsInRole("Admin");

            // Kendi þifresini deðiþtiriyorsa veya adminse izin ver
            if (userIdFromToken != id && !isAdmin)
                return Forbid("Sadece kendi þifrenizi veya admin olarak baþka bir kullanýcýnýn þifresini deðiþtirebilirsiniz.");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("Kullanýcý bulunamadý.");

            user.PasswordHash = PasswordHasher.Hash(newPassword);
            _context.SaveChanges();

            return Ok("Þifre güncellendi.");
        }
    }
}
