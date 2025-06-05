using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectTrackerAPI.Models;
using System.IO;

namespace ProjectTrackerAPI.Data
{
    // Migration (classlarý tarayýp veritabaný oluþturur) veya CLI iþlemleri sýrasýnda DbContext'in nasýl oluþturulacaðýný tanýmlar
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        // Uygulama çalýþmýyorken (örneðin dotnet ef komutlarý sýrasýnda) DbContext örneði üretir
        public AppDbContext CreateDbContext(string[] args)
        {
            // appsettings.json dosyasýný okuyacak bir config oluþtur
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())      // Proje dizinini temel al
                .AddJsonFile("appsettings.json")                  // Konfigürasyon dosyasýný yükle
                .Build();

            // EF Core için DbContext yapýlandýrmasý (SQL Server baðlantýsý dahil)
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Baðlantý dizesini kullanarak veritabanýna baðlan
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            // Hazýr DbContext örneðini döndür
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
