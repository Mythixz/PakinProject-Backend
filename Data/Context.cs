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
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // กำหนดค่าของฟิลด์ประเภท decimal
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.Property(e => e.Balance)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)");
            });
         
            base.OnModelCreating(modelBuilder);
        }
    }
}
