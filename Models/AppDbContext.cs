using Microsoft.EntityFrameworkCore;

namespace Library_Item.Models
{
    public class AppDbContext : DbContext
    {
        // Program.cs'ten gelen SQL Server bağlantısını kabul eden zorunlu metot
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Eğer boş constructor varsa hata vermesin diye bunu da ekliyoruz
        public AppDbContext()
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<User> Users { get; set; } // Veri tabanında "Users" adında bir tablo açar
    }
}