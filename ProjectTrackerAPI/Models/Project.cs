using System.ComponentModel.DataAnnotations;

namespace ProjectTrackerAPI.Models
{
    // Sistem içerisindeki projeleri temsil eden model sınıfı
    public class Project
    {
        // Projenin benzersiz kimliği (primary key)
        public int Id { get; set; }

        // Projenin başlığı (zorunlu alan)
        [Required]
        public string Title { get; set; } = string.Empty;

        // Projenin açıklaması (isteğe bağlı)
        public string Description { get; set; } = string.Empty;

        // Projenin mevcut durumu Bekliyor (varsayılan), Devam Ediyor, Tamamlandı. 
        public string Status { get; set; } = "Bekliyor";

        // Projenin bağlı olduğu müşteri (ilişkili foreign key)
        public int? CustomerId { get; set; }

        // İlişkili müşteri nesnesi (navigation property)
        public Customer? Customer { get; set; }

        // Projeye atanan kullanıcılar (many to many ilişki)
        public List<User> Users { get; set; } = new();
    }
}
