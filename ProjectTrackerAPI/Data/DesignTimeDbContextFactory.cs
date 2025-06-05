using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectTrackerAPI.Models;
using System.IO;

namespace ProjectTrackerAPI.Data
{
    // Migration (classlar� taray�p veritaban� olu�turur) veya CLI i�lemleri s�ras�nda DbContext'in nas�l olu�turulaca��n� tan�mlar
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        // Uygulama �al��m�yorken (�rne�in dotnet ef komutlar� s�ras�nda) DbContext �rne�i �retir
        public AppDbContext CreateDbContext(string[] args)
        {
            // appsettings.json dosyas�n� okuyacak bir config olu�tur
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())      // Proje dizinini temel al
                .AddJsonFile("appsettings.json")                  // Konfig�rasyon dosyas�n� y�kle
                .Build();

            // EF Core i�in DbContext yap�land�rmas� (SQL Server ba�lant�s� dahil)
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Ba�lant� dizesini kullanarak veritaban�na ba�lan
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            // Haz�r DbContext �rne�ini d�nd�r
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
