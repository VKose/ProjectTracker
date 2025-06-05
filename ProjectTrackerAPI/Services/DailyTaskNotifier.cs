using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectTrackerAPI.Data;
using ProjectTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectTrackerAPI.Services
{
    // Arka planda �al��an g�rev bildirim servisi (BackgroundService)
    // Her g�n 09:00'da yakla�an g�revler i�in e-posta bildirimi g�nderir
    public class DailyTaskNotifier : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;  // Scoped servisleri almak i�in
        private readonly ILogger<DailyTaskNotifier> _logger; // Loglama i�in

        public DailyTaskNotifier(IServiceProvider serviceProvider, ILogger<DailyTaskNotifier> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Servisin s�rekli �al��t��� k�s�m
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                // Bildirim g�nderim zaman�: her g�n saat 09:00
                var nextRunTime = DateTime.Today.AddHours(9);

                //var nextRunTime = now.AddSeconds(10); // Sadece test i�in 10 sn sonra
                
                // E�er �u an saat 09:00'u ge�mi�se yar�n �al��t�r
                if (now > nextRunTime)
                    nextRunTime = nextRunTime.AddDays(1);

                // Gecikme s�resini hesapla ve log'a yaz
                var delay = nextRunTime - now;
                _logger.LogInformation($" Bildirim servisi {nextRunTime} tarihinde �al��acak. ({delay.TotalMinutes:F1} dakika sonra)");

                // Hesaplanan s�re kadar bekle
                await Task.Delay(delay, stoppingToken);

                try
                {
                    // Scoped servisleri (DbContext, EmailService) kullanabilmek i�in scope olu�tur
                    using var scope = _serviceProvider.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Veritaban�
                    var mail = scope.ServiceProvider.GetRequiredService<EmailService>();    // Mail g�nderici

                    var tomorrow = DateTime.Today.AddDays(1); // Yar�n

                    // Yar�n teslim tarihi olan g�revleri getir (AssignedUser atanm�� olmal�)
                    var todos = await context.Todos
                        .Include(t => t.AssignedUser)  // G�revi alan kullan�c�
                        .Include(t => t.Project)       // G�revin ait oldu�u proje
                        .Where(t => t.DueDate.Date == tomorrow && t.AssignedUser != null)
                        .ToListAsync();

                    // Her g�rev i�in e-posta g�nder
                    foreach (var todo in todos)
                    {
                        // HTML format�nda mail i�eri�i haz�rla
                        var htmlBody = $@"
                            <h3>Yakla�an G�rev Hat�rlatmas�</h3>
                            <p><strong>Proje:</strong> {todo.Project?.Title ?? "Bilinmiyor"}</p>
                            <p><strong>G�rev:</strong> {todo.Title}</p>
                            <p><strong>Termin:</strong> {todo.DueDate:dd.MM.yyyy}</p>
                            <p><strong>Sorumlu:</strong> {todo.AssignedUser!.Name}</p>
                        ";

                        try
                        {
                            // E-posta g�nder
                            mail.Send(
                                to: todo.AssignedUser.Email,
                                subject: $"Hat�rlatma: {todo.Title} g�revinin teslim tarihi yakla��yor",
                                body: htmlBody,
                                isHtml: true
                            );
                        }
                        catch (Exception emailEx)
                        {
                            // E-posta g�nderilemezse logla
                            _logger.LogError(emailEx, $"E-posta g�nderilemedi: {todo.AssignedUser.Email}");
                        }
                    }

                    // Ba�ar�l� g�nderim logu
                    _logger.LogInformation($" {todos.Count} g�rev i�in e-posta bildirimi g�nderildi.");
                }
                catch (Exception ex)
                {
                    // Genel hata logu
                    _logger.LogError(ex, " G�rev bildirimi s�ras�nda genel bir hata olu�tu.");
                }
            }
        }
    }
}
