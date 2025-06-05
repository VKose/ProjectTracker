using Microsoft.EntityFrameworkCore;
using ProjectTrackerAPI.Models;
using System.Collections.Generic;

namespace ProjectTrackerAPI.Data
{
    // Uygulaman�n veritaban� ba�lam� (DbContext)
    public class AppDbContext : DbContext
    {
        // Yap�land�rma ayarlar�n� constructor ile al�r
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Kullan�c� tablosu (Users)
        public DbSet<User> Users => Set<User>();

        // M��teri tablosu (Customers)
        public DbSet<Customer> Customers => Set<Customer>();

        // Proje tablosu (Projects)
        public DbSet<Project> Projects => Set<Project>();

        // G�rev tablosu (Todos)
        public DbSet<Todo> Todos { get; set; }  
    }
}
