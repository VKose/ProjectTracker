namespace ProjectTrackerAPI.DTOs
{
    // Kullanýcý kayýt iþlemi için kullanýlan DTO (Data Transfer Object)
    // Yalnýzca dýþarýdan alýnmasý gereken verileri içerir
    public class UserRegisterDto
    {
        // Kullanýcýnýn adý
        public string Name { get; set; } = string.Empty;

        // Kullanýcýnýn e-posta adresi
        public string Email { get; set; } = string.Empty;

        // Kullanýcýnýn þifrelenmemiþ parolasý
        public string Password { get; set; } = string.Empty;

        // Kullanýcýnýn rolü: Admin, Manager veya Employee olabilir
        // Varsayýlan olarak "Employee" atanýr
        public string Role { get; set; } = "Employee";
    }
}
