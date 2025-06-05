using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Services
{
    // E-posta gönderim servisi
    public class EmailService
    {
        private readonly MailSettings _mailSettings;

        // Yapýlandýrma dosyasýndan mail ayarlarýný alýr
        public EmailService(IConfiguration config)
        {
            _mailSettings = config.GetSection("MailSettings").Get<MailSettings>()!;
        }

        // E-posta gönderme iþlemi
        public void Send(string to, string subject, string body, bool isHtml = false)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail)); // Gönderici
            email.To.Add(MailboxAddress.Parse(to)); // Alýcý
            email.Subject = subject;

            // HTML, düz metin
            email.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

            using var smtp = new SmtpClient(); // SMTP sunucusuna baðlan
            smtp.Connect(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Username, _mailSettings.Password); //kimlik doðrulama

            // E-postayý gönder ve baðlantýyý kapat
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
