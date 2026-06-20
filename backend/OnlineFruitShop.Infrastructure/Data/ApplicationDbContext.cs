using Microsoft.EntityFrameworkCore;
using OnlineFruitShop.Core.Entities;

namespace OnlineFruitShop.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Fruit> Fruits => Set<Fruit>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<OrderItem>().HasOne(i => i.Fruit).WithMany().HasForeignKey(i => i.FruitId);
            modelBuilder.Entity<OrderItem>().HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId);
            modelBuilder.Entity<Invoice>().HasOne(i => i.Order).WithMany().HasForeignKey(i => i.OrderId);
        }
    }
}
