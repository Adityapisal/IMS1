using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed initial categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Grocery" },
                new Category { Id = 3, Name = "Furniture" },
                new Category { Id = 4, Name = "Clothing" },
                new Category { Id = 5, Name = "Books" }
            );
        }
    }
} 