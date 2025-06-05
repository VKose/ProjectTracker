using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectTrackerAPI.Services
{
    // Arka planda çalýþan görev bildirim servisi (BackgroundService)
    // Her gün 09:00'da yaklaþan görevler için e-posta bildirimi gönderir
    public class DailyTaskNotifier : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;  // Scoped servisleri almak için
        private readonly ILogger<DailyTaskNotifier> _logger; // Loglama için

        public DailyTaskNotifier(IServiceProvider serviceProvider, ILogger<DailyTaskNotifier> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Servisin sürekli çalýþtýðý kýsým
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                // Bildirim gönderim zamaný: her gün saat 09:00
                var nextRunTime = DateTime.Today.AddHours(9);

                //var nextRunTime = now.AddSeconds(10); // Sadece test için 10 sn sonra
                
                // Eðer þu an saat 09:00'u geçmiþse yarýn çalýþtýr
                if (now > nextRunTime)
                    nextRunTime = nextRunTime.AddDays(1);

                // Gecikme süresini hesapla ve log'a yaz
                var delay = nextRunTime - now;
                _logger.LogInformation($" Bildirim servisi {nextRunTime} tarihinde çalýþacak. ({delay.TotalMinutes:F1} dakika sonra)");

                // Hesaplanan süre kadar bekle
                await Task.Delay(delay, stoppingToken);

                try
                {
                    // Scoped servisleri (DbContext, EmailService) kullanabilmek için scope oluþtur
                    using var scope = _serviceProvider.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Veritabaný
                    var mail = scope.ServiceProvider.GetRequiredService<EmailService>();    // Mail gönderici

                    var tomorrow = DateTime.Today.AddDays(1); // Yarýn

                    // Yarýn teslim tarihi olan görevleri getir (AssignedUser atanmýþ olmalý)
                    var todos = await context.Todos
                        .Include(t => t.AssignedUser)  // Görevi alan kullanýcý
                        .Include(t => t.Project)       // Görevin ait olduðu proje
                        .Where(t => t.DueDate.Date == tomorrow && t.AssignedUser != null)
                        .ToListAsync();

                    // Her görev için e-posta gönder
                    foreach (var todo in todos)
                    {
                        // HTML formatýnda mail içeriði hazýrla
                        var htmlBody = $@"
                            <h3>Yaklaþan Görev Hatýrlatmasý</h3>
                            <p><strong>Proje:</strong> {todo.Project?.Title ?? "Bilinmiyor"}</p>
                            <p><strong>Görev:</strong> {todo.Title}</p>
                            <p><strong>Termin:</strong> {todo.DueDate:dd.MM.yyyy}</p>
                            <p><strong>Sorumlu:</strong> {todo.AssignedUser!.Name}</p>
                        ";

                        try
                        {
                            // E-posta gönder
                            mail.Send(
                                to: todo.AssignedUser.Email,
                                subject: $"Hatýrlatma: {todo.Title} görevinin teslim tarihi yaklaþýyor",
                                body: htmlBody,
                                isHtml: true
                            );
                        }
                        catch (Exception emailEx)
                        {
                            // E-posta gönderilemezse logla
                            _logger.LogError(emailEx, $"E-posta gönderilemedi: {todo.AssignedUser.Email}");
                        }
                    }

                    // Baþarýlý gönderim logu
                    _logger.LogInformation($" {todos.Count} görev için e-posta bildirimi gönderildi.");
                }
                catch (Exception ex)
                {
                    // Genel hata logu
                    _logger.LogError(ex, " Görev bildirimi sýrasýnda genel bir hata oluþtu.");
                }
            }
        }
    }
}
