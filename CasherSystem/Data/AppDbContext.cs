using Microsoft.EntityFrameworkCore;
using CasherSystem.Models;

namespace CasherSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> syProducts { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaledProduct> SaledProducts { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Return> Returns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Return>()
                .HasOne(r => r.Sale)
                .WithMany(s => s.Returns)
                .HasForeignKey(r => r.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SaledProduct relationships
            modelBuilder.Entity<SaledProduct>()
                .HasOne(sp => sp.Sale)
                .WithMany(s => s.products)
                .HasForeignKey(sp => sp.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaledProduct>()
                .HasOne(sp => sp.Product)
                .WithMany()
                .HasForeignKey(sp => sp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Return entity has no ProcessedByUserId property, so no relationship configuration needed

            // Configure decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.CostPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.SellPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(s => s.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(s => s.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(s => s.NetTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.PaidAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.RemainingAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Return>()
                .Property(r => r.TotalRefund)
                .HasPrecision(18, 2);
        }

    }
}
