using Microsoft.EntityFrameworkCore;
using ProjectTrackerAPI.Models;
using System.Collections.Generic;

namespace ProjectTrackerAPI.Data
{
    // Uygulamanýn veritabaný baðlamý (DbContext)
    public class AppDbContext : DbContext
    {
        // Yapýlandýrma ayarlarýný constructor ile alýr
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Kullanýcý tablosu (Users)
        public DbSet<User> Users => Set<User>();

        // Müþteri tablosu (Customers)
        public DbSet<Customer> Customers => Set<Customer>();

        // Proje tablosu (Projects)
        public DbSet<Project> Projects => Set<Project>();

        // Görev tablosu (Todos)
        public DbSet<Todo> Todos { get; set; }  
    }
}
