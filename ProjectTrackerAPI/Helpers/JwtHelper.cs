using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Helpers
{
    // JWT token üretimi için yardýmcý sýnýf
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        // Yapýlandýrma ayarlarýný (appsettings.json) al
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        // Belirli bir kullanýcý için JWT token üretir
        public string GenerateToken(User user)
        {
            // appsettings.json -> "Jwt" bölümü içindeki ayarlarý al
            var jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();

            // Null kontrolü
            if (jwtSettings == null ||
                string.IsNullOrWhiteSpace(jwtSettings.Key) ||
                string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
                string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                throw new InvalidOperationException("JWT ayarlarý yapýlandýrma dosyasýnda eksik. 'Jwt:Key', 'Jwt:Issuer' ve 'Jwt:Audience' tanýmlarý yapýlmalýdýr.");
            }

            // Token içerisine eklenecek bilgiler (claims)
            var claims = new[]
            {
                // Kullanýcýnýn benzersiz ID'si
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // Kullanýcýnýn e-posta adresi
                new Claim(ClaimTypes.Email, user.Email),

                // Kullanýcýnýn rolü (Admin, Manager, Employee)
                new Claim(ClaimTypes.Role, user.Role),

                // Kullanýcýnýn adý (frontend için gerekli)
                new Claim(ClaimTypes.Name, user.Name)
            };

            // Þifreleme için anahtar oluþtur (256-bit secret key)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

            // Token imzasý için HMAC-SHA256 algoritmasý kullan
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token nesnesi oluþtur
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,             // Token'ý oluþturan (sunucu adý)
                audience: jwtSettings.Audience,         // Token'ý kullanacak uygulama (API)
                claims: claims,                         // Token'a gömülecek bilgiler
                expires: DateTime.UtcNow.AddHours(1),   // 1 saat geçerli
                signingCredentials: creds               // Ýmza bilgisi
            );

            // Token string olarak geri döndürülür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
