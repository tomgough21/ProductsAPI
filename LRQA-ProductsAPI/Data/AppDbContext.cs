using LRQA_ProductsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LRQA_ProductsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // seed initial data for products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Running Shoes", Price = 129.99m, Stock = 150 },
                new Product { Id = 2, Name = "Walking Boots", Price = 89.99m, Stock = 75 },
                new Product { Id = 2, Name = "Flip Flops", Price = 19.99m, Stock = 45 }
            );
        }
    }
}
