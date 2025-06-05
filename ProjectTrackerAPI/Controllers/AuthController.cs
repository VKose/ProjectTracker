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

        // Kullan�c� Kayd� (Yetkisiz kullan�c�lar eri�ebilir)
        // POST: /api/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // Token gerektirmez
        public IActionResult Register(UserRegisterDto dto)
        {
            // Ayn� e-posta ile kay�tl� kullan�c� var m� kontrol et
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta zaten kay�tl�.");

            // Yeni kullan�c� olu�tur
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kay�t ba�ar�l�.");
        }

        // Giri� ��lemi (Yetkisiz kullan�c�lar eri�ebilir)
        // POST: /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous] // Token gerektirmez
        public IActionResult Login(UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            // Kullan�c� yoksa ya da �ifre yanl��sa hata ver
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("E-posta veya �ifre hatal�.");

            // Token �ret ve g�nder
            var token = _jwtHelper.GenerateToken(user);
            return Ok(new { token });
        }

        // �ifre De�i�tir (Sadece yetkili kullan�c�lar eri�ebilir)
        // PUT: /api/auth/change-password/{id}
        [HttpPut("change-password/{id}")]
        [Authorize] // Giri� yap�lm�� olmal�
        public IActionResult ChangePassword(int id, [FromBody] string newPassword)
        {
            // Token'dan giri� yapan ki�inin ID'sini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Token'dan ID al�namazsa veya parse edilemezse hata
            if (!int.TryParse(userIdClaim, out int userIdFromToken))
                return Unauthorized("Ge�ersiz kullan�c� kimli�i.");

            // Admin mi kontrol et
            var isAdmin = User.IsInRole("Admin");

            // Kendi �ifresini de�i�tiriyorsa veya adminse izin ver
            if (userIdFromToken != id && !isAdmin)
                return Forbid("Sadece kendi �ifrenizi veya admin olarak ba�ka bir kullan�c�n�n �ifresini de�i�tirebilirsiniz.");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("Kullan�c� bulunamad�.");

            user.PasswordHash = PasswordHasher.Hash(newPassword);
            _context.SaveChanges();

            return Ok("�ifre g�ncellendi.");
        }
    }
}
