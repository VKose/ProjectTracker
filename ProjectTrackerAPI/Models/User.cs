using ProjectTrackerAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

// Sistem i�indeki kullan�c�lar� temsil eden model s�n�f�
public class User
{
    // Kullan�c�n�n benzersiz kimli�i (primary key)
    public int Id { get; set; }

    // Kullan�c�n�n ad�
    public string Name { get; set; } = "";

    // Kullan�c�n�n e-posta adresi (giri� i�in kullan�l�r)
    public string Email { get; set; } = "";

    // �ifrelenmi� parola (hash'lenmi� olarak saklan�r)
    public string PasswordHash { get; set; } = "";

    // Kullan�c�n�n rol�: Admin, Manager veya Employee
    public string Role { get; set; } = "";

    [NotMapped]
    public string Password { get; set; } = "";

    // Kullan�c�n�n dahil oldu�u projeler (many to many ili�ki)
    // Bu ili�ki Project.cs i�indeki List<User> ile e�le�ir
    public List<Project> Projects { get; set; } = new();
}
