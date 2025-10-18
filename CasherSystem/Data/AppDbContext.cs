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

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<UserInfo>().HasData(
                new UserInfo
                {
                    Id = 1,
                    Username = "admin",
                    PhoneNumber = "01234567890"
                },
                new UserInfo
                {
                    Id = 2,
                    Username = "seller1",
                    PhoneNumber = "01234567891"
                },
                new UserInfo
                {
                    Id = 3,
                    Username = "seller2",
                    PhoneNumber = "01234567892"
                }
            );

            // Seed Products with Arabic names
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "قميص قطني",
                    Barcode = "TSH001",
                    Size = "M",
                    Color = "أزرق",
                    CostPrice = 15.00m,
                    SellPrice = 25.00m,
                    Quantity = 50,
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "جينز",
                    Barcode = "JEA001",
                    Size = "L",
                    Color = "أزرق",
                    CostPrice = 35.00m,
                    SellPrice = 60.00m,
                    Quantity = 30,
                    IsActive = true
                },
                new Product
                {
                    Id = 3,
                    Name = "هودي",
                    Barcode = "HOO001",
                    Size = "XL",
                    Color = "أسود",
                    CostPrice = 40.00m,
                    SellPrice = 75.00m,
                    Quantity = 20,
                    IsActive = true
                },
                new Product
                {
                    Id = 4,
                    Name = "فستان صيفي",
                    Barcode = "DRE001",
                    Size = "S",
                    Color = "أحمر",
                    CostPrice = 25.00m,
                    SellPrice = 45.00m,
                    Quantity = 15,
                    IsActive = true
                },
                new Product
                {
                    Id = 5,
                    Name = "حذاء رياضي",
                    Barcode = "SNE001",
                    Size = "42",
                    Color = "أبيض",
                    CostPrice = 50.00m,
                    SellPrice = 90.00m,
                    Quantity = 25,
                    IsActive = true
                },
                new Product
                {
                    Id = 6,
                    Name = "قميص رسمي",
                    Barcode = "SHI001",
                    Size = "L",
                    Color = "أبيض",
                    CostPrice = 30.00m,
                    SellPrice = 55.00m,
                    Quantity = 40,
                    IsActive = true
                },
                new Product
                {
                    Id = 7,
                    Name = "بنطلون كاجوال",
                    Barcode = "PAN001",
                    Size = "M",
                    Color = "رمادي",
                    CostPrice = 45.00m,
                    SellPrice = 80.00m,
                    Quantity = 35,
                    IsActive = true
                },
                new Product
                {
                    Id = 8,
                    Name = "جاكيت شتوي",
                    Barcode = "JAC001",
                    Size = "XL",
                    Color = "بني",
                    CostPrice = 80.00m,
                    SellPrice = 150.00m,
                    Quantity = 12,
                    IsActive = true
                }
            );
        }
    }
}
