namespace ProjectTrackerAPI.DTOs
{
    // Kullanýcý giriþinde kullanýlan veri transfer nesnesi (DTO)
    // Sadece gerekli alanlarý içerir (þifre hash'lenmeden gelir)
    public class UserLoginDto
    {
        // Kullanýcýnýn giriþ yaparken yazdýðý e-posta adresi
        public string Email { get; set; } = string.Empty;

        // Kullanýcýnýn giriþ için girdiði þifrelenmemiþ parola
        public string Password { get; set; } = string.Empty;
    }
}
