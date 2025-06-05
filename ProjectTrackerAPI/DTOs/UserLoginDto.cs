namespace ProjectTrackerAPI.DTOs
{
    // Kullan�c� giri�inde kullan�lan veri transfer nesnesi (DTO)
    // Sadece gerekli alanlar� i�erir (�ifre hash'lenmeden gelir)
    public class UserLoginDto
    {
        // Kullan�c�n�n giri� yaparken yazd��� e-posta adresi
        public string Email { get; set; } = string.Empty;

        // Kullan�c�n�n giri� i�in girdi�i �ifrelenmemi� parola
        public string Password { get; set; } = string.Empty;
    }
}
