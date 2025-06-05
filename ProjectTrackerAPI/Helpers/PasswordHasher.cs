using System.Security.Cryptography;
using System.Text;

namespace ProjectTrackerAPI.Helpers
{
    // Parola g�venli�i i�in kullan�lan yard�mc� s�n�f
    // �ifreleri SHA-256 algoritmas� ile hash'ler ve do�rular
    public static class PasswordHasher
    {
        // Parolay� SHA-256 algoritmas�yla hash'ler
        public static string Hash(string password)
        {
            // SHA-256 algoritma nesnesi olu�tur
            using var sha = SHA256.Create();

            // Parolay� byte dizisine d�n��t�r
            var bytes = Encoding.UTF8.GetBytes(password);

            // Hash i�lemini uygula
            var hash = sha.ComputeHash(bytes);

            // Hash sonucunu base64 format�nda string'e �evir ve d�nd�r
            return Convert.ToBase64String(hash);
        }

        // Girilen parola ile kay�tl� hash kar��la�t�r�l�r
        public static bool Verify(string input, string hash)
        {
            // Girilen parola yeniden hash'lenir ve e�itlik kontrol� yap�l�r
            return Hash(input) == hash;
        }
    }
}
