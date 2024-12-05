using Microsoft.EntityFrameworkCore;
using PakinProject.Models;

namespace PakinProject.Data
{
    public class PakinProjectContext : DbContext
    {
        public PakinProjectContext(DbContextOptions<PakinProjectContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; } // เพิ่ม DbSet สำหรับ OrderItem
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                // เปลี่ยนจาก TotalAmount เป็น TotalPrice
                entity.Property(e => e.TotalPrice)
                      .HasColumnType("decimal(18,2)");
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}