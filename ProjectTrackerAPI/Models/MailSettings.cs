namespace ProjectTrackerAPI.Models
{
    // E-posta gönderimi için gerekli SMTP ayarlarýný temsil eden sýnýf
    // Bu ayarlar appsettings.json dosyasýndan okunur
    public class MailSettings
    {
        // SMTP sunucusunun adresi (örnek: smtp.gmail.com, smtp.mailtrap.io biz mailtrap kullandýk)
        public string SmtpServer { get; set; } = "";

        // SMTP sunucusunun port numarasý (örnek: 587)
        public int SmtpPort { get; set; }

        // Gönderenin görünen adý (örnek: Project Tracker)
        public string SenderName { get; set; } = "";

        // Gönderen e-posta adresi (örnek: noreply@projecttracker.com)
        public string SenderEmail { get; set; } = "";

        // SMTP kullanýcý adý (MailTrap veya Gmail'de genellikle e-posta adresi ya da özel kullanýcý adý)
        public string Username { get; set; } = "";

        // SMTP þifresi
        public string Password { get; set; } = "";
    }
}
