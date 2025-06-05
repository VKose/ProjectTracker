using System.Security.Cryptography;
using System.Text;

namespace ProjectTrackerAPI.Helpers
{
    // Parola güvenliði için kullanýlan yardýmcý sýnýf
    // Þifreleri SHA-256 algoritmasý ile hash'ler ve doðrular
    public static class PasswordHasher
    {
        // Parolayý SHA-256 algoritmasýyla hash'ler
        public static string Hash(string password)
        {
            // SHA-256 algoritma nesnesi oluþtur
            using var sha = SHA256.Create();

            // Parolayý byte dizisine dönüþtür
            var bytes = Encoding.UTF8.GetBytes(password);

            // Hash iþlemini uygula
            var hash = sha.ComputeHash(bytes);

            // Hash sonucunu base64 formatýnda string'e çevir ve döndür
            return Convert.ToBase64String(hash);
        }

        // Girilen parola ile kayýtlý hash karþýlaþtýrýlýr
        public static bool Verify(string input, string hash)
        {
            // Girilen parola yeniden hash'lenir ve eþitlik kontrolü yapýlýr
            return Hash(input) == hash;
        }
    }
}
