namespace ProjectTrackerAPI.DTOs
{
    // Kullan�c� kay�t i�lemi i�in kullan�lan DTO (Data Transfer Object)
    // Yaln�zca d��ar�dan al�nmas� gereken verileri i�erir
    public class UserRegisterDto
    {
        // Kullan�c�n�n ad�
        public string Name { get; set; } = string.Empty;

        // Kullan�c�n�n e-posta adresi
        public string Email { get; set; } = string.Empty;

        // Kullan�c�n�n �ifrelenmemi� parolas�
        public string Password { get; set; } = string.Empty;

        // Kullan�c�n�n rol�: Admin, Manager veya Employee olabilir
        // Varsay�lan olarak "Employee" atan�r
        public string Role { get; set; } = "Employee";
    }
}
