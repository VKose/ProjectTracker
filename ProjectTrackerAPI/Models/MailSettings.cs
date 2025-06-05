namespace ProjectTrackerAPI.Models
{
    // E-posta g�nderimi i�in gerekli SMTP ayarlar�n� temsil eden s�n�f
    // Bu ayarlar appsettings.json dosyas�ndan okunur
    public class MailSettings
    {
        // SMTP sunucusunun adresi (�rnek: smtp.gmail.com, smtp.mailtrap.io biz mailtrap kulland�k)
        public string SmtpServer { get; set; } = "";

        // SMTP sunucusunun port numaras� (�rnek: 587)
        public int SmtpPort { get; set; }

        // G�nderenin g�r�nen ad� (�rnek: Project Tracker)
        public string SenderName { get; set; } = "";

        // G�nderen e-posta adresi (�rnek: noreply@projecttracker.com)
        public string SenderEmail { get; set; } = "";

        // SMTP kullan�c� ad� (MailTrap veya Gmail'de genellikle e-posta adresi ya da �zel kullan�c� ad�)
        public string Username { get; set; } = "";

        // SMTP �ifresi
        public string Password { get; set; } = "";
    }
}
