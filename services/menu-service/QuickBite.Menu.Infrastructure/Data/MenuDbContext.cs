using Microsoft.EntityFrameworkCore;
using QuickBite.Menu.Domain.Entities;

namespace QuickBite.Menu.Infrastructure.Data
{
    // DbContext is the main EF Core class that manages database tables
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {
        }

        // Table for menu categories
        public DbSet<MenuCategory> MenuCategories { get; set; }

        // Table for menu items
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary key for MenuCategory
            modelBuilder.Entity<MenuCategory>()
                .HasKey(category => category.Id);

            // Validation for category name
            modelBuilder.Entity<MenuCategory>()
                .Property(category => category.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Validation for category description
            modelBuilder.Entity<MenuCategory>()
                .Property(category => category.Description)
                .HasMaxLength(300);

            // Primary key for MenuItem
            modelBuilder.Entity<MenuItem>()
                .HasKey(item => item.Id);

            // Validation for item name
            modelBuilder.Entity<MenuItem>()
                .Property(item => item.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Validation for item description
            modelBuilder.Entity<MenuItem>()
                .Property(item => item.Description)
                .HasMaxLength(500);

            // Decimal precision for item price
            modelBuilder.Entity<MenuItem>()
                .Property(item => item.Price)
                .HasColumnType("decimal(18,2)");

            // One category can have many items
            modelBuilder.Entity<MenuItem>()
                .HasOne(item => item.MenuCategory)
                .WithMany(category => category.MenuItems)
                .HasForeignKey(item => item.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}