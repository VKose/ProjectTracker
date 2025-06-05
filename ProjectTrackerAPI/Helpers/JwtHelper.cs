using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Helpers
{
    // JWT token �retimi i�in yard�mc� s�n�f
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        // Yap�land�rma ayarlar�n� (appsettings.json) al
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        // Belirli bir kullan�c� i�in JWT token �retir
        public string GenerateToken(User user)
        {
            // appsettings.json -> "Jwt" b�l�m� i�indeki ayarlar� al
            var jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();

            // Null kontrol�
            if (jwtSettings == null ||
                string.IsNullOrWhiteSpace(jwtSettings.Key) ||
                string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
                string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                throw new InvalidOperationException("JWT ayarlar� yap�land�rma dosyas�nda eksik. 'Jwt:Key', 'Jwt:Issuer' ve 'Jwt:Audience' tan�mlar� yap�lmal�d�r.");
            }

            // Token i�erisine eklenecek bilgiler (claims)
            var claims = new[]
            {
                // Kullan�c�n�n benzersiz ID'si
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // Kullan�c�n�n e-posta adresi
                new Claim(ClaimTypes.Email, user.Email),

                // Kullan�c�n�n rol� (Admin, Manager, Employee)
                new Claim(ClaimTypes.Role, user.Role),

                // Kullan�c�n�n ad� (frontend i�in gerekli)
                new Claim(ClaimTypes.Name, user.Name)
            };

            // �ifreleme i�in anahtar olu�tur (256-bit secret key)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

            // Token imzas� i�in HMAC-SHA256 algoritmas� kullan
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token nesnesi olu�tur
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,             // Token'� olu�turan (sunucu ad�)
                audience: jwtSettings.Audience,         // Token'� kullanacak uygulama (API)
                claims: claims,                         // Token'a g�m�lecek bilgiler
                expires: DateTime.UtcNow.AddHours(1),   // 1 saat ge�erli
                signingCredentials: creds               // �mza bilgisi
            );

            // Token string olarak geri d�nd�r�l�r
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
