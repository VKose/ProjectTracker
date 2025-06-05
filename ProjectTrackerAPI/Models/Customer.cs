using System.Collections.Generic;

namespace ProjectTrackerAPI.Models
{
    // Sistemdeki müşterileri temsil eden model sınıfı
    // Müşteriler, projelerle ilişkilidir (bir müşteri birden fazla projeye sahip olabilir)
    public class Customer
    {
        // Her müşterinin benzersiz kimliği (primary key)
        public int Id { get; set; }

        // Müşteri adı 
        public string Name { get; set; } = string.Empty;

        // Müşteri iletişim bilgileri (e-posta)
        public string ContactInfo { get; set; } = string.Empty;

        // Bu müşteriye ait projeler (ilişki)
        public List<Project>? Projects { get; set; }
    }
}
