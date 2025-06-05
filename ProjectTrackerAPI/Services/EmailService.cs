using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProjectTrackerAPI.Models;

namespace ProjectTrackerAPI.Services
{
    // E-posta g�nderim servisi
    public class EmailService
    {
        private readonly MailSettings _mailSettings;

        // Yap�land�rma dosyas�ndan mail ayarlar�n� al�r
        public EmailService(IConfiguration config)
        {
            _mailSettings = config.GetSection("MailSettings").Get<MailSettings>()!;
        }

        // E-posta g�nderme i�lemi
        public void Send(string to, string subject, string body, bool isHtml = false)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail)); // G�nderici
            email.To.Add(MailboxAddress.Parse(to)); // Al�c�
            email.Subject = subject;

            // HTML, d�z metin
            email.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

            using var smtp = new SmtpClient(); // SMTP sunucusuna ba�lan
            smtp.Connect(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Username, _mailSettings.Password); //kimlik do�rulama

            // E-postay� g�nder ve ba�lant�y� kapat
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
