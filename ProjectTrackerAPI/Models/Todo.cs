using System.ComponentModel.DataAnnotations;

namespace ProjectTrackerAPI.Models
{
    // Sistem içindeki yapılacak görevleri temsil eden model sınıfı
    public class Todo
    {
        // Görevin benzersiz kimliği (primary key)
        public int Id { get; set; }

        // Görevin başlığı (zorunlu)
        [Required]
        public string Title { get; set; } = string.Empty;

        // Görev açıklaması
        public string Description { get; set; } = string.Empty;

        // Görevin mevcut durumu, varsayılan olarak "Bekliyor", Devam Ediyor, Tamamlandı, İptal Edildi
        public string Status { get; set; } = "Bekliyor";

        // Görevin teslim edilmesi gereken tarih
        public DateTime DueDate { get; set; }

        // Görev bir projeye bağlı olabilir (foreign key)
        public int? ProjectId { get; set; }

        // Görevle ilişkili proje nesnesi (navigation property)
        public Project? Project { get; set; }

        // Göreve atanan kullanıcı ID'si (nullable)
        public int? AssignedUserId { get; set; }

        // Atanan kullanıcı nesnesi (navigation property)
        public User? AssignedUser { get; set; }
    }
}
