using ProjectTrackerAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

// Sistem içindeki kullanýcýlarý temsil eden model sýnýfý
public class User
{
    // Kullanýcýnýn benzersiz kimliði (primary key)
    public int Id { get; set; }

    // Kullanýcýnýn adý
    public string Name { get; set; } = "";

    // Kullanýcýnýn e-posta adresi (giriþ için kullanýlýr)
    public string Email { get; set; } = "";

    // Þifrelenmiþ parola (hash'lenmiþ olarak saklanýr)
    public string PasswordHash { get; set; } = "";

    // Kullanýcýnýn rolü: Admin, Manager veya Employee
    public string Role { get; set; } = "";

    [NotMapped]
    public string Password { get; set; } = "";

    // Kullanýcýnýn dahil olduðu projeler (many to many iliþki)
    // Bu iliþki Project.cs içindeki List<User> ile eþleþir
    public List<Project> Projects { get; set; } = new();
}
